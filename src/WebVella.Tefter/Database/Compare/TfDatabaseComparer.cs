namespace WebVella.Tefter.Database;

internal static class TfDatabaseComparer
{
    public static TfDifferenceCollection Compare(TfDatabaseTableCollection initialCollection, TfDatabaseTableCollection modifiedCollection)
    {
        TfDifferenceCollection differences = new TfDifferenceCollection();

        if ((initialCollection == null || initialCollection.Count() == 0) &&
            (modifiedCollection == null || modifiedCollection.Count() == 0))
        {
            return differences;
        }

        //all tables are removed
        if (initialCollection != null && (modifiedCollection == null || modifiedCollection.Count() == 0))
        {
            foreach (TfDatabaseTable table in initialCollection)
            {
                var diffs = Compare(table, null);
                differences.AddRange(diffs);
            }
        }
        //all tables are new
        else if ((initialCollection == null || initialCollection.Count() == 0) && modifiedCollection != null)
        {
            foreach (TfDatabaseTable table in modifiedCollection)
            {
                var diffs = Compare(null, table);
                differences.AddRange(diffs);
            }
        }
        else
        {
            HashSet<Guid> processedTables = new HashSet<Guid>();
            foreach (TfDatabaseTable table in initialCollection)
            {
                processedTables.Add(table.Id);
                var modifiedTable = modifiedCollection.Find(table.Id);

                if (modifiedTable != null && modifiedTable.Name != table.Name)
                {
                    differences.Add(new TfDifference
                    {
                        Type = TfDifferenceActionType.Error,
                        ObjectType = TfDifferenceObjectType.Table,
                        Object = table,
                        ObjectName = table.Name,
                        TableName = table.Name,
                        Descriptions = new List<string> {
                            $"Attempt to compare tables with same id {table.Id}, but different names ('{table.Name}' <> '{modifiedTable.Name}'). " +
                            $"Table name changes are not supported. These two tables will not be compared. "
                        }.AsReadOnly()
                    });

                    continue;
                }

                var diffs = Compare(table, modifiedTable);
                differences.AddRange(diffs);
            }
            foreach (TfDatabaseTable table in modifiedCollection)
            {
                //if table is already processed, skip it
                if (processedTables.Contains(table.Id))
                    continue;
                //if table is not processed, it is new 
                var diffs = Compare(null, table);
                differences.AddRange(diffs);
            }
        }
        
        return Process(differences);
    }

    private static TfDifferenceCollection Compare(TfDatabaseTable initialTable, TfDatabaseTable modifiedTable)
    {
        TfDifferenceCollection differences = new TfDifferenceCollection();

        if (initialTable == null && modifiedTable == null)
            return differences;

        //table is removed
        if (initialTable != null && modifiedTable == null)
        {
            //remove table will remove all related columns, indexes and constraints
            //so no need to generate remove for them
            differences.Add(new TfDifference
            {
                Type = TfDifferenceActionType.Remove,
                ObjectType = TfDifferenceObjectType.Table,
                TableName = initialTable.Name,
                ObjectName = initialTable.Name,
                Object = initialTable,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(initialTable)} '{initialTable.Name}' will be removed" }.AsReadOnly()
            });
        }
        //table is added
        else if (initialTable == null && modifiedTable != null)
        {
            //add new table difference record
            differences.Add(new TfDifference
            {
                Type = TfDifferenceActionType.Add,
                ObjectType = TfDifferenceObjectType.Table,
                TableName = modifiedTable.Name,
                ObjectName = modifiedTable.Name,
                Object = modifiedTable,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedTable)} '{modifiedTable.Name}' will be added" }.AsReadOnly()
            });

            #region <--- indexes --->
            {
                foreach (var index in modifiedTable.Indexes)
                {
                    var diffs = Compare(modifiedTable.Name, null, index);
                    differences.AddRange(diffs);
                }
            }
            #endregion

            #region <--- constraints --->
            {
                foreach (var constraint in modifiedTable.Constraints)
                {
                    var diffs = Compare(modifiedTable.Name, null, constraint);
                    differences.AddRange(diffs);
                }
            }
            #endregion

            #region <--- columns --->
            {
                foreach (var column in modifiedTable.Columns)
                {
                    var diffs = Compare(modifiedTable.Name, null, column);
                    differences.AddRange(diffs);
                }
            }
            #endregion
        }
        //both tables exists
        else
        {
            #region <--- indexes --->
            {
                HashSet<string> processedIndexes = new HashSet<string>();
                foreach (var initialIndex in initialTable.Indexes)
                {
                    processedIndexes.Add(initialIndex.Name);
                    var modifiedIndex = modifiedTable.Indexes.Find(initialIndex.Name);
                    var diffs = Compare(initialTable.Name, initialIndex, modifiedIndex);
                    differences.AddRange(diffs);
                }
                foreach (var modifiedIndex in modifiedTable.Indexes)
                {
                    //if index is already processed, skip it
                    if (processedIndexes.Contains(modifiedIndex.Name))
                        continue;

                    //if index is not processed, it is new 
                    var diffs = Compare(initialTable.Name, null, modifiedIndex);
                    differences.AddRange(diffs);
                }
            }

            #endregion

            #region <--- constraints --->
            {
                HashSet<string> processedConstraints = new HashSet<string>();
                foreach (var initialConstraint in initialTable.Constraints)
                {
                    processedConstraints.Add(initialConstraint.Name);
                    var modifiedConstraint = modifiedTable.Constraints.Find(initialConstraint.Name);
                    var diffs = Compare(initialTable.Name, initialConstraint, modifiedConstraint);
                    differences.AddRange(diffs);
                }
                foreach (var modifiedConstraint in modifiedTable.Constraints)
                {
                    //if constraint is already processed, skip it
                    if (processedConstraints.Contains(modifiedConstraint.Name))
                        continue;

                    //if constraint is not processed, it is new 
                    var diffs = Compare(modifiedTable.Name, null, modifiedConstraint);
                    differences.AddRange(diffs);
                }
            }
            #endregion

            #region <--- columns --->
            {
                HashSet<string> processedColumns = new HashSet<string>();
                foreach (var initialColumn in initialTable.Columns)
                {
                    processedColumns.Add(initialColumn.Name);
                    var modifiedColumn = modifiedTable.Columns.Find(initialColumn.Id);
                    var diffs = Compare(initialTable.Name, initialColumn, modifiedColumn);
                    differences.AddRange(diffs);
                }
                foreach (var modifiedColumn in modifiedTable.Columns)
                {
                    //if column is already processed, skip it
                    if (processedColumns.Contains(modifiedColumn.Name))
                        continue;

                    //if column is not processed, it is new 
                    var diffs = Compare(modifiedTable.Name, null, modifiedColumn);
                    differences.AddRange(diffs);
                }
            }
            #endregion
        }

        return differences;
    }

    private static TfDifferenceCollection Compare(string tableName, TfDatabaseColumn initialColumn, TfDatabaseColumn modifiedColumn)
    {
        TfDifferenceCollection differences = new TfDifferenceCollection();
        if (initialColumn == null && modifiedColumn == null)
            return differences;

        //column is removed
        if (initialColumn != null && modifiedColumn == null)
        {
            differences.Add(new TfDifference
            {
                Type = TfDifferenceActionType.Remove,
                TableName = tableName,
                ObjectType = TfDifferenceObjectType.Column,
                ObjectName = initialColumn.Name,
                Object = initialColumn,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(initialColumn)} '{initialColumn.Name}' will be removed." }.AsReadOnly()
            });
        }
        //column is added
        else if (initialColumn == null && modifiedColumn != null)
        {
            differences.Add(new TfDifference
            {
                Type = TfDifferenceActionType.Add,
                TableName = tableName,
                ObjectType = TfDifferenceObjectType.Column,
                ObjectName = modifiedColumn.Name,
                Object = modifiedColumn,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedColumn)} '{modifiedColumn.Name}' will be added." }.AsReadOnly()
            });
        }
        //both columns exists
        else
        {
            if (initialColumn.Type != modifiedColumn.Type)
            {
                differences.Add(new TfDifference
                {
                    Type = TfDifferenceActionType.Error,
                    ObjectType = TfDifferenceObjectType.Column,
                    TableName = tableName,
                    Object = modifiedColumn,
                    ObjectName = modifiedColumn.Name,
                    Descriptions = new List<string> {
                        $"Attempt to update of columns with same id({modifiedColumn.Id}) from type '{GetDbObjectTypeName(initialColumn)}' to '{GetDbObjectTypeName(modifiedColumn)}'. " +
                        $"It's not supported. These columns will not be compared."
                    }.AsReadOnly()
                });

                return differences;
            }

            if (initialColumn.Name != modifiedColumn.Name)
            {
                differences.Add(new TfDifference
                {
                    Type = TfDifferenceActionType.Error,
                    ObjectType = TfDifferenceObjectType.Column,
                    TableName = tableName,
                    ObjectName = modifiedColumn.Name,
                    Object = modifiedColumn,
                    Descriptions = new List<string> {
                        $"Attempt to update of column with same id({modifiedColumn.Id}) from name '{initialColumn.Name}' to '{modifiedColumn.Name}'. " +
                        $"It's not supported. These columns will not be compared."
                    }.AsReadOnly()
                });

                return differences;
            }

			if (string.IsNullOrWhiteSpace( initialColumn.GeneratedExpression ) != string.IsNullOrWhiteSpace( modifiedColumn.GeneratedExpression ) )
			{
				if (string.IsNullOrWhiteSpace(initialColumn.GeneratedExpression))
				{
					differences.Add(new TfDifference
					{
						Type = TfDifferenceActionType.Error,
						ObjectType = TfDifferenceObjectType.Column,
						TableName = tableName,
						ObjectName = modifiedColumn.Name,
						Object = modifiedColumn,
						Descriptions = new List<string> {
						$"Attempt to update regular column and make it generated."+
						$"It's not supported. These columns will not be compared."
					}.AsReadOnly()
					});
				}
				else
				{
					differences.Add(new TfDifference
					{
						Type = TfDifferenceActionType.Error,
						ObjectType = TfDifferenceObjectType.Column,
						TableName = tableName,
						ObjectName = modifiedColumn.Name,
						Object = modifiedColumn,
						Descriptions = new List<string> {
						$"Attempt to update generated column and make it regular column."+
						$"It's not supported. These columns will not be compared."
					}.AsReadOnly()
					});
				}

				return differences;
			}

			List<string> descriptions = new List<string>();

			if (initialColumn.GeneratedExpression != null )
			{
				if (initialColumn.GeneratedExpression != modifiedColumn.GeneratedExpression)
				{
					descriptions.Add($"{GetDbObjectTypeName(modifiedColumn)} '{modifiedColumn.Name}' will change generation expression to '{modifiedColumn.GeneratedExpression}'");
				}
			}
			else
			{
				if (initialColumn.IsNullable != modifiedColumn.IsNullable)
				{
					if (modifiedColumn.IsNullable)
						descriptions.Add($"{GetDbObjectTypeName(modifiedColumn)} '{modifiedColumn.Name}' will be made NULLABLE");
					else
						descriptions.Add($"{GetDbObjectTypeName(modifiedColumn)} '{modifiedColumn.Name}' will be made NOT NULLABLE");
				}

				if (!AreDefaultValuesEqual(initialColumn.DefaultValue, modifiedColumn.DefaultValue))
				{
					if (modifiedColumn.DefaultValue == null)
						descriptions.Add($"{GetDbObjectTypeName(modifiedColumn)} '{modifiedColumn.Name}' default value be changed to NULL");
					else
						descriptions.Add($"{GetDbObjectTypeName(modifiedColumn)} '{modifiedColumn.Name}' default value be changed to '{modifiedColumn.DefaultValue}'");
				}

				if (initialColumn.GetType().IsAssignableFrom(typeof(TfDatabaseColumnWithAutoDefaultValue)))
				{
					var initialAutoDefaultValue = ((TfDatabaseColumnWithAutoDefaultValue)initialColumn).AutoDefaultValue;
					var modifiedAutoDefaultValue = ((TfDatabaseColumnWithAutoDefaultValue)modifiedColumn).AutoDefaultValue;
					if (initialAutoDefaultValue != modifiedAutoDefaultValue)
					{
						if (modifiedAutoDefaultValue)
							descriptions.Add($"{GetDbObjectTypeName(initialColumn)} '{initialColumn.Name}' automatic generation of default value will be switched ON.");
						else
							descriptions.Add($"{GetDbObjectTypeName(initialColumn)} '{initialColumn.Name}' automatic generation of default value will be switched OFF.");
					}
				}
			}

            if (descriptions.Count > 0)
            {
                differences.Add(new TfDifference
                {
                    Type = TfDifferenceActionType.Update,
                    TableName = tableName,
                    ObjectType = TfDifferenceObjectType.Column,
                    Object = modifiedColumn,
                    ObjectName = modifiedColumn.Name,
                    Descriptions = descriptions.AsReadOnly()
                });
            }
        }

        return differences;
    }

    private static TfDifferenceCollection Compare(string tableName, TfDatabaseIndex initialIndex, TfDatabaseIndex modifiedIndex)
    {
        TfDifferenceCollection differences = new TfDifferenceCollection();
        if (initialIndex == null && modifiedIndex == null)
            return differences;

        //index is removed
        if (initialIndex != null && modifiedIndex == null)
        {
            differences.Add(new TfDifference
            {
                Type = TfDifferenceActionType.Remove,
                TableName = tableName,
                ObjectName = initialIndex.Name,
                ObjectType = TfDifferenceObjectType.Index,
                Object = initialIndex,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(initialIndex)} '{initialIndex.Name}' will be removed." }.AsReadOnly()
            });
        }
        //index is added
        else if (initialIndex == null && modifiedIndex != null)
        {
            differences.Add(new TfDifference
            {
                Type = TfDifferenceActionType.Add,
                TableName = tableName,
                ObjectName = modifiedIndex.Name,
                ObjectType = TfDifferenceObjectType.Index,
                Object = modifiedIndex,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedIndex)} '{modifiedIndex.Name}' will be added." }.AsReadOnly()
            });
        }
        //both indexes exists
        else
        {
            if (initialIndex.GetType() != modifiedIndex.GetType())
            {
                differences.Add(new TfDifference
                {
                    Type = TfDifferenceActionType.Remove,
                    TableName = tableName,
                    ObjectName = initialIndex.Name,
                    ObjectType = TfDifferenceObjectType.Index,
                    Object = initialIndex,
                    Descriptions = new List<string> { $"{GetDbObjectTypeName(initialIndex)} '{initialIndex.Name}' will be removed because type of the index was changed." }.AsReadOnly()
                });

                differences.Add(new TfDifference
                {
                    Type = TfDifferenceActionType.Add,
                    TableName = tableName,
                    ObjectName = modifiedIndex.Name,
                    ObjectType = TfDifferenceObjectType.Index,
                    Object = modifiedIndex,
                    Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedIndex)} '{modifiedIndex.Name}' will be added." }.AsReadOnly()
                });
            }
            else
            {
                bool columnsAreChanged = false;

                if (initialIndex.Columns.Count() != modifiedIndex.Columns.Count())
                {
                    columnsAreChanged = true;
                }
                else
                {
                    for (int i = 0; i < initialIndex.Columns.Count(); i++)
                    {
                        if (initialIndex.Columns[i] != modifiedIndex.Columns[i])
                        {
                            columnsAreChanged = true;
                            break;
                        }
                    }
                }

                if (columnsAreChanged)
                {
                    differences.Add(new TfDifference
                    {
                        Type = TfDifferenceActionType.Remove,
                        TableName = tableName,
                        ObjectName = initialIndex.Name,
                        ObjectType = TfDifferenceObjectType.Index,
                        Object = initialIndex,
                        Descriptions = new List<string> { $"{GetDbObjectTypeName(initialIndex)} '{initialIndex.Name}' will be removed because index columns are changed." }.AsReadOnly()
                    });

                    differences.Add(new TfDifference
                    {
                        Type = TfDifferenceActionType.Add,
                        TableName = tableName,
                        ObjectName = modifiedIndex.Name,
                        ObjectType = TfDifferenceObjectType.Index,
                        Object = modifiedIndex,
                        Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedIndex)} '{modifiedIndex.Name}' will be added." }.AsReadOnly()
                    });
                }
            }
        }

        return differences;
    }

    private static TfDifferenceCollection Compare(string tableName, TfDatabaseConstraint initialConstraint, TfDatabaseConstraint modifiedConstraint)
    {
        TfDifferenceCollection differences = new TfDifferenceCollection();
        if (initialConstraint == null && modifiedConstraint == null)
            return differences;

        //constraint is removed
        if (initialConstraint != null && modifiedConstraint == null)
        {
            differences.Add(new TfDifference
            {
                Type = TfDifferenceActionType.Remove,
                TableName = tableName,
                ObjectName = initialConstraint.Name,
                ObjectType = TfDifferenceObjectType.Constraint,
                Object = initialConstraint,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(initialConstraint)} '{initialConstraint.Name}' will be removed." }.AsReadOnly()
            });
        }
        //constraint is added
        else if (initialConstraint == null && modifiedConstraint != null)
        {
            differences.Add(new TfDifference
            {
                Type = TfDifferenceActionType.Add,
                TableName = tableName,
                ObjectName = modifiedConstraint.Name,
                ObjectType = TfDifferenceObjectType.Constraint,
                Object = modifiedConstraint,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedConstraint)} '{modifiedConstraint.Name}' will be added." }.AsReadOnly()
            });
        }
        //both constraints exists
        else
        {
            if (initialConstraint.GetType() != modifiedConstraint.GetType())
            {
                differences.Add(new TfDifference
                {
                    Type = TfDifferenceActionType.Remove,
                    TableName = tableName,
                    ObjectName = initialConstraint.Name,
                    ObjectType = TfDifferenceObjectType.Constraint,
                    Object = initialConstraint,
                    Descriptions = new List<string> { $"{GetDbObjectTypeName(initialConstraint)} '{initialConstraint.Name}' will be removed because type of the constraint was changed." }.AsReadOnly()
                });

                differences.Add(new TfDifference
                {
                    Type = TfDifferenceActionType.Add,
                    TableName = tableName,
                    ObjectName = modifiedConstraint.Name,
                    ObjectType = TfDifferenceObjectType.Constraint,
                    Object = modifiedConstraint,
                    Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedConstraint)} '{modifiedConstraint.Name}' will be added." }.AsReadOnly()
                });
            }
            else
            {
                bool columnsAreChanged = false;

                if (initialConstraint.Columns.Count() != modifiedConstraint.Columns.Count())
                {
                    columnsAreChanged = true;
                }
                else
                {
                    for (int i = 0; i < initialConstraint.Columns.Count(); i++)
                    {
                        if (initialConstraint.Columns[i] != modifiedConstraint.Columns[i])
                        {
                            columnsAreChanged = true;
                            break;
                        }
                    }
                }

                if (columnsAreChanged)
                {
                    differences.Add(new TfDifference
                    {
                        Type = TfDifferenceActionType.Remove,
                        TableName = tableName,
                        ObjectName = initialConstraint.Name,
                        ObjectType = TfDifferenceObjectType.Constraint,
                        Object = initialConstraint,
                        Descriptions = new List<string> { $"{GetDbObjectTypeName(initialConstraint)} '{initialConstraint.Name}' will be removed because constraint columns are changed." }.AsReadOnly()
                    });

                    differences.Add(new TfDifference
                    {
                        Type = TfDifferenceActionType.Add,
                        TableName = tableName,
                        ObjectName = modifiedConstraint.Name,
                        ObjectType = TfDifferenceObjectType.Constraint,
                        Object = modifiedConstraint,
                        Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedConstraint)} '{modifiedConstraint.Name}' will be added." }.AsReadOnly()
                    });
                }
                else
                {
                    if (initialConstraint.GetType() == typeof(TfDatabaseForeignKeyConstraint))
                    {
                        var initialFkConstraint = (TfDatabaseForeignKeyConstraint)initialConstraint;
                        var modifiedFkConstraint = (TfDatabaseForeignKeyConstraint)modifiedConstraint;

                        if (initialFkConstraint.ForeignTable != modifiedFkConstraint.ForeignTable)
                        {
                            differences.Add(new TfDifference
                            {
                                Type = TfDifferenceActionType.Remove,
                                TableName = tableName,
                                ObjectName = initialConstraint.Name,
                                ObjectType = TfDifferenceObjectType.Constraint,
                                Object = initialConstraint,
                                Descriptions = new List<string> { $"{GetDbObjectTypeName(initialConstraint)} '{initialConstraint.Name}' will be removed because foreign table is changed." }.AsReadOnly()
                            });

                            differences.Add(new TfDifference
                            {
                                Type = TfDifferenceActionType.Add,
                                TableName = tableName,
                                ObjectName = modifiedConstraint.Name,
                                ObjectType = TfDifferenceObjectType.Constraint,
                                Object = modifiedConstraint,
                                Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedConstraint)} '{modifiedConstraint.Name}' will be added." }.AsReadOnly()
                            });
                        }
                        else
                        {
                            bool fkColumnsAreChanged = false;

                            if (initialFkConstraint.ForeignColumns.Count() != modifiedFkConstraint.ForeignColumns.Count())
                            {
                                fkColumnsAreChanged = true;
                            }
                            else
                            {
                                for (int i = 0; i < initialFkConstraint.ForeignColumns.Count(); i++)
                                {
                                    if (initialFkConstraint.ForeignColumns[i] != modifiedFkConstraint.ForeignColumns[i])
                                    {
                                        fkColumnsAreChanged = true;
                                        break;
                                    }
                                }
                            }

                            if (fkColumnsAreChanged)
                            {
                                differences.Add(new TfDifference
                                {
                                    Type = TfDifferenceActionType.Remove,
                                    TableName = tableName,
                                    ObjectName = initialConstraint.Name,
                                    ObjectType = TfDifferenceObjectType.Constraint,
                                    Object = initialConstraint,
                                    Descriptions = new List<string> { $"{GetDbObjectTypeName(initialConstraint)} '{initialConstraint.Name}' will be removed foreign columns is changed." }.AsReadOnly()
                                });

                                differences.Add(new TfDifference
                                {
                                    Type = TfDifferenceActionType.Add,
                                    TableName = tableName,
                                    ObjectName = modifiedConstraint.Name,
                                    ObjectType = TfDifferenceObjectType.Constraint,
                                    Object = modifiedConstraint,
                                    Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedConstraint)} '{modifiedConstraint.Name}' will be added." }.AsReadOnly()
                                });
                            }
                        }
                    }
                }
            }
        }

        return differences;
    }

    private static string GetDbObjectTypeName(TfDatabaseObject obj)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");

        switch (obj)
        {
            case TfDatabaseTable: return "table";
            case TfAutoIncrementDatabaseColumn: return "auto increment column";
            case TfBooleanDatabaseColumn: return "boolean column";
            case TfDateDatabaseColumn: return "Date column";
            case TfDateTimeDatabaseColumn: return "DateTime column";
            case TfGuidDatabaseColumn: return "Guid column";
            case TfNumberDatabaseColumn: return "number column";
			case TfShortIntegerDatabaseColumn: return "short integer column";
			case TfIntegerDatabaseColumn: return "integer column";
			case TfLongIntegerDatabaseColumn: return "long integer column";
			case TfTextDatabaseColumn: return "text column";
			case TfShortTextDatabaseColumn: return "short text column";
			case TfDatabaseForeignKeyConstraint: return "foreign key constraint";
            case TfDatabasePrimaryKeyConstraint: return "primary key constraint";
            case TfDatabaseUniqueKeyConstraint: return "unique key constraint";
            case TfBTreeDatabaseIndex: return "btree index";
            case TfGinDatabaseIndex: return "gin index";
            case TfGistDatabaseIndex: return "gist index";
            case TfHashDatabaseIndex: return "hash index";

            default:
                throw new TfDatabaseException($"Not supported object type: {obj.GetType()}");
        }
    }

    private static bool AreDefaultValuesEqual(object obj1, object obj2)
    {
        if (obj1 == null && obj2 == null)
            return true;
        if (obj1 == null && obj2 != null)
            return false;
        if (obj1 != null && obj2 == null)
            return false;

        if(obj1.GetType() != obj2.GetType())
            return false;
        
        //because we know that our default values are not complex objects,
        //we can compare them by .ToString()
        return obj1.ToString().Equals(obj2.ToString());
    }

    //remove duplicates and sort in order for SQL script generation
    private static TfDifferenceCollection Process(TfDifferenceCollection differences)
    {
        TfDifferenceCollection result = new TfDifferenceCollection();

        //remove duplicated records
        Dictionary<string, TfDifference> dict = new Dictionary<string, TfDifference>();
        foreach (var diff in differences)
        {
            string key = $"{diff.Type}{diff.ObjectType}{diff.TableName}{diff.ObjectName}";
            if (!dict.ContainsKey(key))
                dict[key] = diff;
        }

        List<TfDifference> filteredList = dict.Values.ToList();

        //remove index
        result.AddRange(filteredList
            .Where(x => x.Type == TfDifferenceActionType.Remove &&
                    x.ObjectType == TfDifferenceObjectType.Index));

        //remove constraints
        result.AddRange(filteredList
            .Where(x => x.Type == TfDifferenceActionType.Remove &&
                    x.ObjectType == TfDifferenceObjectType.Constraint));

        //remove columns
        result.AddRange(filteredList
            .Where(x => x.Type == TfDifferenceActionType.Remove &&
                    x.ObjectType == TfDifferenceObjectType.Column));

        //remove tables
        result.AddRange(filteredList
            .Where(x => x.Type == TfDifferenceActionType.Remove &&
                x.ObjectType == TfDifferenceObjectType.Table));

        //add tables
        result.AddRange(filteredList
          .Where(x => x.Type == TfDifferenceActionType.Add &&
                  x.ObjectType == TfDifferenceObjectType.Table));

        //add columns
        result.AddRange(filteredList
          .Where(x => x.Type == TfDifferenceActionType.Add &&
                  x.ObjectType == TfDifferenceObjectType.Column));

        //add indexes
        result.AddRange(filteredList
          .Where(x => x.Type == TfDifferenceActionType.Add &&
                  x.ObjectType == TfDifferenceObjectType.Index));

        //add constraints
        result.AddRange(filteredList
          .Where(x => x.Type == TfDifferenceActionType.Add &&
                  x.ObjectType == TfDifferenceObjectType.Constraint));

        //update columns
        result.AddRange(filteredList
          .Where(x => x.Type == TfDifferenceActionType.Update &&
                  x.ObjectType == TfDifferenceObjectType.Column));

        if (filteredList.Count != result.Count)
            throw new TfDatabaseException("Process of processing differences result with incorrect count. This should not happen.");
            
        return result;
    }
}
