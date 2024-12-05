﻿using Nito.AsyncEx.Synchronous;

namespace WebVella.Tefter;

public partial interface ITfSpaceManager
{
	public Result<List<TfSpaceNode>> GetAllSpaceNodes();

	public Result<List<TfSpaceNode>> GetSpaceNodes(Guid spaceId);

	public Result<TfSpaceNode> GetSpaceNode(Guid nodeId);

	public Result<(Guid, List<TfSpaceNode>)> CreateSpaceNode(
		TfSpaceNode spaceNode);

	public Result<List<TfSpaceNode>> UpdateSpaceNode(
		TfSpaceNode spaceNode);

	public Result<List<TfSpaceNode>> DeleteSpaceNode(
		TfSpaceNode spaceNode);

	public Result<(Guid, List<TfSpaceNode>)> CopySpaceNode(
		Guid nodeId);
}

public partial class TfSpaceManager : ITfSpaceManager
{
	public Result<List<TfSpaceNode>> GetAllSpaceNodes()
	{
		try
		{
			var spaceNodesList = _dboManager.GetList<TfSpaceNodeDbo>()
				.Select(x => Convert(x))
				.ToList();

			var rootNodes = spaceNodesList
				.Where(x => x.ParentId is null)
				.OrderBy(x => x.Position)
				.ToList();

			foreach (var rootNode in rootNodes)
			{
				rootNode.ParentNode = null;
				InitSpaceNodeChildNodes(rootNode, spaceNodesList);
			}

			return Result.Ok(rootNodes);

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get list of space nodes").CausedBy(ex));
		}
	}

	public Result<List<TfSpaceNode>> GetSpaceNodes(
		Guid spaceId)
	{
		try
		{
			var spaceNodesList = _dboManager.GetList<TfSpaceNodeDbo>(
					spaceId,
					nameof(TfSpaceView.SpaceId))
				.Select(x => Convert(x))
				.ToList();

			var rootNodes = spaceNodesList
				.Where(x => x.ParentId is null)
				.OrderBy(x => x.Position)
				.ToList();

			foreach (var rootNode in rootNodes)
			{
				rootNode.ParentNode = null;
				InitSpaceNodeChildNodes(rootNode, spaceNodesList);
			}

			return Result.Ok(rootNodes);

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get list of space nodes").CausedBy(ex));
		}
	}

	public Result<TfSpaceNode> GetSpaceNode(Guid nodeId)
	{
		try
		{
			var allNodesResult = GetAllSpaceNodes();

			if (allNodesResult.IsFailed)
			{
				return Result.Fail(new Error("GetAllSpaceNodes failed")
						.CausedBy(allNodesResult.Errors));
			}

			var node = FindNodeById(nodeId, allNodesResult.Value);

			return Result.Ok(node);

		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to get space node").CausedBy(ex));
		}
	}
	private void InitSpaceNodeChildNodes(
		TfSpaceNode node,
		List<TfSpaceNode> allNodes)
	{
		var childNodes = allNodes
			.Where(x => x.ParentId == node.Id)
			.OrderBy(x => x.Position)
			.ToList();

		node.ChildNodes.AddRange(childNodes);

		foreach (var childNode in childNodes)
		{
			childNode.ParentNode = node;
			InitSpaceNodeChildNodes(childNode, allNodes);
		}
	}

	private TfSpaceNode FindNodeById(
		Guid id,
		List<TfSpaceNode> nodes)
	{
		if (nodes == null || nodes.Count == 0)
			return null;

		Queue<TfSpaceNode> queue = new Queue<TfSpaceNode>();

		foreach (var node in nodes)
		{
			if (node.Id == id)
				return node;

			queue.Enqueue(node);
		}

		while (queue.Count > 0)
		{
			var node = queue.Dequeue();
			if (node.Id == id)
				return node;

			foreach (var childNode in node.ChildNodes)
				queue.Enqueue(childNode);
		}

		return null;
	}

	public Result<(Guid, List<TfSpaceNode>)> CreateSpaceNode(
		TfSpaceNode spaceNode)
	{
		try
		{
			if (spaceNode != null && spaceNode.Id == Guid.Empty)
				spaceNode.Id = Guid.NewGuid();

			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{

				var allNodes = GetSpaceNodes(spaceNode.SpaceId).Value;

				TfSpaceNodeValidator validator = new TfSpaceNodeValidator(allNodes);

				var validationResult = validator.ValidateCreate(spaceNode);

				if (!validationResult.IsValid)
					return validationResult.ToResult();


				List<TfSpaceNode> nodesToUpdate = new List<TfSpaceNode>();

				TfSpaceNode parentNode = null;
				short? maxPosition = null;
				if (spaceNode.ParentId is null)
				{
					maxPosition = allNodes
						.Where(x => x.ParentId is null)
						.Select(x => (short?)x.Position.Value)
						.Max(x => x);
				}
				else
				{
					parentNode = FindNodeById(spaceNode.ParentId.Value, allNodes);

					maxPosition = parentNode
						.ChildNodes
						.Select(x => (short?)x.Position.Value)
						.Max(x => x);
				}
				if (maxPosition is null)
					maxPosition = 0;

				//if position is not specified or bigger than max position, we add node at the end
				if (spaceNode.Position == null || spaceNode.Position > maxPosition)
				{
					spaceNode.Position = (short)(maxPosition + 1);
				}
				//if new position is equal or lower to 0 and current position is 1, 
				// we do not change position
				else if (spaceNode.Position <= 0)
				{
					spaceNode.Position = 1;
				}

				var childNodesForPositionUpdate = new List<TfSpaceNode>();

				if (parentNode is not null)
				{
					childNodesForPositionUpdate = parentNode.ChildNodes
						.Where(x => x.Position.Value >= spaceNode.Position)
						.ToList();
				}
				else
				{
					childNodesForPositionUpdate = allNodes
						.Where(x => x.ParentId is null && x.Position.Value >= spaceNode.Position)
						.ToList();
				}

				foreach (var childNode in childNodesForPositionUpdate)
				{
					childNode.Position++;
					nodesToUpdate.Add(childNode);
				}


				//update nodes which position is changed
				foreach (var nodeToUpdate in nodesToUpdate)
				{
					if (!_dboManager.Update<TfSpaceNodeDbo>(Convert(nodeToUpdate), nameof(TfSpaceNodeDbo.Position)))
						return Result.Fail(new DboManagerError("Update", nodeToUpdate));
				}

				if (!_dboManager.Insert<TfSpaceNodeDbo>(Convert(spaceNode)))
					return Result.Fail(new DboManagerError("Insert", spaceNode));

				if (spaceNode.Type == TfSpaceNodeType.Page && !String.IsNullOrWhiteSpace(spaceNode.ComponentTypeFullName))
				{
					var spaceNodeComponents = _metaProvider.GetSpaceNodesComponentsMeta();
					var nodeComponent = spaceNodeComponents.SingleOrDefault(x => x.ComponentType.FullName == spaceNode.ComponentTypeFullName);
					if (nodeComponent is not null)
					{
						var task = Task.Run(async () =>
						{
							var context = new TfSpaceNodeComponentContext
							{
								SpaceId = spaceNode.SpaceId,
								SpaceNodeId = spaceNode.Id,
								ComponentOptionsJson = spaceNode.ComponentOptionsJson,
								Icon = spaceNode.Icon,
								Mode = TfComponentMode.Update
							};

							var optionsJson = await nodeComponent.Instance.OnNodeCreated(_serviceProvider, context);
							if (optionsJson != spaceNode.ComponentOptionsJson)
							{
								spaceNode.ComponentOptionsJson = optionsJson;
								if (!_dboManager.Update<TfSpaceNodeDbo>(Convert(spaceNode), nameof(TfSpaceNodeDbo.ComponentSettingsJson)))
									throw new Exception("Space Node update failed");
							}
						});
						task.WaitAndUnwrapException();
					}
				}

				scope.Complete();

				allNodes = GetSpaceNodes(spaceNode.SpaceId).Value;

				return Result.Ok((spaceNode.Id, allNodes));
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to create space node").CausedBy(ex));
		}
	}

	public Result<List<TfSpaceNode>> UpdateSpaceNode(
		TfSpaceNode spaceNode)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var allNodes = GetSpaceNodes(spaceNode.SpaceId).Value.ToList();

				TfSpaceNodeValidator validator = new TfSpaceNodeValidator(allNodes);

				var validationResult = validator.ValidateUpdate(spaceNode);

				if (!validationResult.IsValid)
					return validationResult.ToResult();

				var existingSpaceNode = FindNodeById(spaceNode.Id, allNodes);

				List<TfSpaceNode> nodesToUpdate = new List<TfSpaceNode>();

				nodesToUpdate.Add(spaceNode);

				if (spaceNode.ParentId != existingSpaceNode.ParentId)
				{
					//update existing node parent children nodes position
					if (existingSpaceNode.ParentId is not null)
					{
						var childNodesForPositionUpdate = new List<TfSpaceNode>();

						existingSpaceNode.ParentNode.ChildNodes.Remove(existingSpaceNode);

						childNodesForPositionUpdate = existingSpaceNode.ParentNode.ChildNodes
							.Where(x => x.Position.Value > existingSpaceNode.Position)
							.ToList();

						foreach (var childNode in childNodesForPositionUpdate)
						{
							childNode.Position--;
							if (!nodesToUpdate.Any(x => x.Id == childNode.Id))
								nodesToUpdate.Add(childNode);
						}
					}
					else
					{
						var childNodesForPositionUpdate = new List<TfSpaceNode>();

						allNodes.Remove(existingSpaceNode);

						childNodesForPositionUpdate = allNodes
							.Where(x => x.ParentId is null && x.Position.Value > existingSpaceNode.Position)
							.ToList();

						foreach (var childNode in childNodesForPositionUpdate)
						{
							childNode.Position--;
							if (!nodesToUpdate.Any(x => x.Id == childNode.Id))
								nodesToUpdate.Add(childNode);
						}
					}

					TfSpaceNode parentNode = null;
					short? maxPosition = null;
					if (spaceNode.ParentId is null)
					{
						maxPosition = allNodes
							.Where(x => x.ParentId is null)
							.Select(x => (short?)x.Position.Value)
							.Max(x => x);
					}
					else
					{
						parentNode = FindNodeById(spaceNode.ParentId.Value, allNodes);

						maxPosition = parentNode
							.ChildNodes
							.Select(x => (short?)x.Position.Value)
							.Max(x => x);
					}
					if (maxPosition is null)
						maxPosition = 0;

					//if position is not valid, we add node at the end
					if (spaceNode.Position == null || spaceNode.Position <= 0 || spaceNode.Position > maxPosition)
					{
						spaceNode.Position = (short)(maxPosition + 1);
					}
					else
					{
						//else move nodes after position we insert at with one position down
						var childNodesForPositionUpdate = new List<TfSpaceNode>();

						if (parentNode is null)
						{
							allNodes.Remove(existingSpaceNode);

							childNodesForPositionUpdate = allNodes
								.Where(x => x.Position.Value >= spaceNode.Position)
								.ToList();
						}
						else
						{
							parentNode.ChildNodes.Remove(existingSpaceNode);

							childNodesForPositionUpdate = parentNode.ChildNodes
								.Where(x => x.Position.Value >= spaceNode.Position)
								.ToList();
						}

						foreach (var childNode in childNodesForPositionUpdate)
						{
							childNode.Position++;
							if (!nodesToUpdate.Any(x => x.Id == childNode.Id))
								nodesToUpdate.Add(childNode);
						}
					}

				}
				else if (spaceNode.Position != existingSpaceNode.Position)
				{

					TfSpaceNode parentNode = null;
					if (spaceNode.ParentId is not null)
						parentNode = FindNodeById(spaceNode.ParentId.Value, allNodes);

					var childNodesForPositionDecrease = new List<TfSpaceNode>();

					short? maxPosition = null;
					if (spaceNode.ParentId is null)
					{
						allNodes.Remove(existingSpaceNode);

						maxPosition = allNodes
							.Where(x => x.ParentId is null)
							.Select(x => (short?)x.Position.Value)
							.Max(x => x);
					}
					else
					{
						parentNode.ChildNodes.Remove(existingSpaceNode);

						maxPosition = parentNode
							.ChildNodes
							.Select(x => (short?)x.Position.Value)
							.Max(x => x);
					}
					if (maxPosition is null)
						maxPosition = 1;

					//if position is not valid, we add node at the end
					if (spaceNode.Position == null || spaceNode.Position > maxPosition)
					{
						spaceNode.Position = (short)(maxPosition);
					}
					//if new position is equal or lower to 0 and current position is 1, 
					// we do not change position
					else if (spaceNode.Position <= 0 && existingSpaceNode.Position == 1)
					{
						spaceNode.Position = 1;
					}

					//only calculate if position is changed
					if (spaceNode.Position != existingSpaceNode.Position)
					{
						if (parentNode is not null)
						{
							childNodesForPositionDecrease = parentNode.ChildNodes
								.Where(x => x.Position.Value >= existingSpaceNode.Position)
								.ToList();

							foreach (var childNode in childNodesForPositionDecrease)
							{
								childNode.Position--;
								if (!nodesToUpdate.Any(x => x.Id == childNode.Id))
									nodesToUpdate.Add(childNode);
							}
						}
						else
						{
							childNodesForPositionDecrease = allNodes
								.Where(x => x.ParentId is null && x.Position.Value >= existingSpaceNode.Position)
								.ToList();

							foreach (var childNode in childNodesForPositionDecrease)
							{
								childNode.Position--;
								if (!nodesToUpdate.Any(x => x.Id == childNode.Id))
									nodesToUpdate.Add(childNode);
							}
						}

						var childNodesForPositionIncrease = new List<TfSpaceNode>();

						if (parentNode is not null)
						{
							parentNode.ChildNodes.Remove(existingSpaceNode);

							childNodesForPositionIncrease = parentNode.ChildNodes
								.Where(x => x.Position.Value >= spaceNode.Position)
								.ToList();

							foreach (var childNode in childNodesForPositionIncrease)
							{
								childNode.Position++;
								if (!nodesToUpdate.Any(x => x.Id == childNode.Id))
									nodesToUpdate.Add(childNode);
							}

						}
						else
						{
							allNodes.Remove(existingSpaceNode);

							childNodesForPositionIncrease = allNodes
								.Where(x => x.ParentId is null && x.Position.Value >= spaceNode.Position)
								.ToList();

							foreach (var childNode in childNodesForPositionIncrease)
							{
								childNode.Position++;
								if (!nodesToUpdate.Any(x => x.Id == childNode.Id))
									nodesToUpdate.Add(childNode);
							}
						}
					}
				}

				//update nodes which parent or position is/are changed
				foreach (var childNode in nodesToUpdate)
				{
					if (!_dboManager.Update<TfSpaceNodeDbo>(Convert(childNode)))
						return Result.Fail(new DboManagerError("Update", childNode));
				}


				if (spaceNode.Type == TfSpaceNodeType.Page && !String.IsNullOrWhiteSpace(spaceNode.ComponentTypeFullName))
				{
					var spaceNodeComponents = _metaProvider.GetSpaceNodesComponentsMeta();
					var nodeComponent = spaceNodeComponents.SingleOrDefault(x => x.ComponentType.FullName == spaceNode.ComponentTypeFullName);
					if (nodeComponent is not null)
					{
						var task = Task.Run(async () =>
						{
							var context = new TfSpaceNodeComponentContext
							{
								SpaceId = spaceNode.SpaceId,
								SpaceNodeId = spaceNode.Id,
								ComponentOptionsJson = spaceNode.ComponentOptionsJson,
								Icon = spaceNode.Icon,
								Mode = TfComponentMode.Update
							};

							var optionsJson = await nodeComponent.Instance.OnNodeUpdated(_serviceProvider, context);
							if (optionsJson != spaceNode.ComponentOptionsJson)
							{
								spaceNode.ComponentOptionsJson = optionsJson;
								if (!_dboManager.Update<TfSpaceNodeDbo>(Convert(spaceNode), nameof(TfSpaceNodeDbo.ComponentSettingsJson)))
									throw new Exception("Space Node update failed");
							}
						});
						task.WaitAndUnwrapException();
					}
				}


				scope.Complete();

				allNodes = GetSpaceNodes(spaceNode.SpaceId).Value;

				return Result.Ok(allNodes);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to update space node").CausedBy(ex));
		}

	}

	public Result<List<TfSpaceNode>> DeleteSpaceNode(
		TfSpaceNode spaceNode)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var allNodes = GetSpaceNodes(spaceNode.SpaceId).Value.ToList();

				TfSpaceNodeValidator validator =
					new TfSpaceNodeValidator(allNodes);

				var validationResult = validator.ValidateDelete(spaceNode);

				if (!validationResult.IsValid)
					return validationResult.ToResult();

				TfSpaceNode parentNode = null;

				if (spaceNode.ParentId.HasValue)
					parentNode = FindNodeById(spaceNode.ParentId.Value, allNodes);

				var childNodesForPositionUpdate = new List<TfSpaceNode>();

				if (parentNode is not null)
				{
					childNodesForPositionUpdate = parentNode.ChildNodes
						.Where(x => x.Position.Value > spaceNode.Position)
						.ToList();
				}
				else
				{
					childNodesForPositionUpdate = allNodes
						.Where(x => x.ParentId is null && x.Position.Value > spaceNode.Position)
						.ToList();
				}

				foreach (var childNode in childNodesForPositionUpdate)
				{
					childNode.Position--;

					if (!_dboManager.Update<TfSpaceNodeDbo>(Convert(childNode), nameof(TfSpaceNodeDbo.Position)))
						return Result.Fail(new DboManagerError("Update", childNode));

				}

				List<TfSpaceNode> nodesToDelete = new List<TfSpaceNode>();
				Queue<TfSpaceNode> queue = new Queue<TfSpaceNode>();

				var node = FindNodeById(spaceNode.Id, allNodes);

				queue.Enqueue(node);

				while (queue.Count > 0)
				{
					var queueNode = queue.Dequeue();
					foreach (var childNode in queueNode.ChildNodes)
						queue.Enqueue(childNode);

					nodesToDelete.Add(queueNode);
				}

				nodesToDelete.Reverse();

				var spaceNodeComponents = _metaProvider.GetSpaceNodesComponentsMeta();

				foreach (var nodeToDelete in nodesToDelete)
				{
					if (!_dboManager.Delete<TfSpaceNodeDbo>(nodeToDelete.Id))
						return Result.Fail(new DboManagerError("Delete", nodeToDelete));

					if (nodeToDelete.Type == TfSpaceNodeType.Page && !String.IsNullOrWhiteSpace(nodeToDelete.ComponentTypeFullName))
					{
						var nodeComponent = spaceNodeComponents.SingleOrDefault(x => x.ComponentType.FullName == nodeToDelete.ComponentTypeFullName);
						if (nodeComponent is not null)
						{
							var task = Task.Run(async () =>
							{
								var context = new TfSpaceNodeComponentContext
								{
									SpaceId = nodeToDelete.SpaceId,
									SpaceNodeId = nodeToDelete.Id,
									ComponentOptionsJson = nodeToDelete.ComponentOptionsJson,
									Icon = nodeToDelete.Icon,
									Mode = TfComponentMode.Update
								};

								await nodeComponent.Instance.OnNodeDeleted(_serviceProvider, context);
							});
							task.WaitAndUnwrapException();
						}
					}

				}

				scope.Complete();

				allNodes = GetSpaceNodes(spaceNode.SpaceId).Value;

				return Result.Ok(allNodes);
			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete space node.").CausedBy(ex));
		}
	}

	public Result<(Guid, List<TfSpaceNode>)> CopySpaceNode(
		Guid nodeId)
	{
		try
		{
			using (var scope = _dbService.CreateTransactionScope(Constants.DB_OPERATION_LOCK_KEY))
			{
				var allNodes = GetAllSpaceNodes().Value.ToList();

				var nodeToCopy = FindNodeById(nodeId, allNodes);

				if (nodeToCopy is null)
				{
					return Result.Fail(new Error("Node not found"));
				}

				List<TfSpaceNode> nodesToCreate = new List<TfSpaceNode>();

				Queue<TfSpaceNode> queue = new Queue<TfSpaceNode>();

				//node is placed after node we copy from
				nodeToCopy.Position++;

				queue.Enqueue(nodeToCopy);

				while (queue.Count > 0)
				{
					var queuedNode = queue.Dequeue();
					queuedNode.Id = Guid.NewGuid();
					nodesToCreate.Add(queuedNode);

					foreach (var childNode in queuedNode.ChildNodes)
					{
						childNode.ParentId = queuedNode.Id;
						queue.Enqueue(childNode);
					}
				}

				foreach (var nodeToCreate in nodesToCreate)
				{
					var createResult = CreateSpaceNode(nodeToCreate);

					if (!createResult.IsSuccess)
					{
						return Result.Fail(createResult.Errors);
					}
				}

				scope.Complete();

				return (nodeToCopy.Id, GetSpaceNodes(nodeToCopy.SpaceId).Value);

			}
		}
		catch (Exception ex)
		{
			return Result.Fail(new Error("Failed to delete space node.").CausedBy(ex));
		}
	}


	private TfSpaceNode Convert(
		TfSpaceNodeDbo dbo)
	{
		if (dbo == null)
			return null;

		var componentType = _metaProvider.GetTypeForFullClassName(dbo.ComponentType);

		return new TfSpaceNode
		{
			Id = dbo.Id,
			Name = dbo.Name,
			Position = dbo.Position,
			SpaceId = dbo.SpaceId,
			Type = dbo.Type,
			ComponentOptionsJson = dbo.ComponentSettingsJson,
			ComponentTypeFullName = dbo.ComponentType,
			ComponentType = componentType,
			Icon = dbo.Icon,
			ParentId = dbo.ParentId
		};
	}

	private TfSpaceNodeDbo Convert(
		TfSpaceNode model)
	{
		if (model == null)
			return null;

		return new TfSpaceNodeDbo
		{
			Id = model.Id,
			Name = model.Name,
			Position = model.Position ?? 0,
			SpaceId = model.SpaceId,
			Type = model.Type,
			ParentId = model.ParentId,
			Icon = model.Icon ?? string.Empty,
			ComponentType = model.ComponentTypeFullName ?? "",
			ComponentSettingsJson = model.ComponentOptionsJson ?? "{}",
		};
	}

	#region <--- validation --->

	internal class TfSpaceNodeValidator
	: AbstractValidator<TfSpaceNode>
	{
		private readonly List<TfSpaceNode> _allNodes;
		public TfSpaceNodeValidator(
			List<TfSpaceNode> allNodes)
		{
			_allNodes = allNodes;

			RuleSet("general", () =>
			{
				RuleFor(spaceNode => spaceNode.Id)
					.NotEmpty()
					.WithMessage("The space node id is required.");

				RuleFor(spaceNode => spaceNode.Name)
					.NotEmpty()
					.WithMessage("The space node name is required.");

				RuleFor(spaceNode => spaceNode.SpaceId)
					.NotEmpty()
					.WithMessage("The space id is required.");
			});

			RuleSet("create", () =>
			{
				RuleFor(spaceNode => spaceNode.Id)
						.Must((spaceNode, id) => { return FindNodeById(id, allNodes) == null; })
						.WithMessage("There is already existing space node with specified identifier.");

				RuleFor(spaceNode => spaceNode.ParentId)
						.Must((spaceNode, parentId) => { return spaceNode.Id != parentId; })
						.WithMessage("Space node cannot be parent to itself.");
			});

			RuleSet("update", () =>
			{
				RuleFor(spaceNode => spaceNode.Id)
						.Must((spaceNode, id) => { return FindNodeById(id, allNodes) != null; })
						.WithMessage("There is not existing space node with specified identifier.");

				RuleFor(spaceNode => spaceNode.ParentId)
						.Must((spaceNode, parentId) => { return spaceNode.Id != parentId; })
						.WithMessage("Space node cannot be parent to itself.");

				RuleFor(spaceNode => spaceNode.ParentId)
						.Must((spaceNode, parentId) =>
						{
							var existingNode = FindNodeById(spaceNode.Id, allNodes);

							return existingNode
									.GetChildNodesPlainList()
									.Count(x => x.ParentId == parentId) == 0;
						})
						.WithMessage("Space node cannot be moved inside its child tree.");

			});

			RuleSet("delete", () =>
			{
			});

		}

		public ValidationResult ValidateCreate(
			TfSpaceNode spaceNode)
		{
			if (spaceNode == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space node is null.") });

			return this.Validate(spaceNode, options =>
			{
				options.IncludeRuleSets("general", "create");
			});
		}

		public ValidationResult ValidateUpdate(
			TfSpaceNode spaceNode)
		{
			if (spaceNode == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space node is null.") });

			return this.Validate(spaceNode, options =>
			{
				options.IncludeRuleSets("general", "update");
			});
		}

		public ValidationResult ValidateDelete(
			TfSpaceNode spaceNode)
		{
			if (spaceNode == null)
				return new ValidationResult(new[] { new ValidationFailure("",
					"The space node is null.") });

			return this.Validate(spaceNode, options =>
			{
				options.IncludeRuleSets("delete");
			});
		}

		private TfSpaceNode FindNodeById(
			Guid id,
			List<TfSpaceNode> nodes)
		{
			if (nodes == null || nodes.Count == 0)
				return null;

			Queue<TfSpaceNode> queue = new Queue<TfSpaceNode>();

			foreach (var node in nodes)
			{
				if (node.Id == id)
					return node;

				queue.Enqueue(node);
			}

			while (queue.Count > 0)
			{
				var node = queue.Dequeue();
				if (node.Id == id)
					return node;

				foreach (var childNode in node.ChildNodes)
					queue.Enqueue(childNode);
			}

			return null;
		}
	}

	#endregion

}
