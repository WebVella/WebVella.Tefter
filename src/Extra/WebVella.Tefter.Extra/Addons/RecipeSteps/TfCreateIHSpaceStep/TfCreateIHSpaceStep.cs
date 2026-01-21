using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using WebVella.Tefter.Assets.Addons;
using WebVella.Tefter.Assets.Services;
using WebVella.Tefter.Database;
using WebVella.Tefter.DataProviders.Excel;
using WebVella.Tefter.UI.Addons;
using WebVella.Tefter.EmailSender.Addons;
using WebVella.Tefter.EmailSender.Models;
using WebVella.Tefter.Talk;
using WebVella.Tefter.Talk.Services;
using WebVella.Tefter.Utility;

namespace WebVella.Tefter.Extra.Addons;

public class TfCreateIHSpaceStep : ITfRecipeStepAddon
{
    //addon
    public Guid AddonId { get; init; } = new Guid("AB2BD6A4-BA92-4D55-A18F-BF962F1EBF02");
    public string AddonName { get; init; } = "IH001 Space";
    public string AddonDescription { get; init; } = "creates a IH Process space based on specifications";
    public string AddonFluentIconName { get; init; } = "PuzzlePiece";
    public Type FormComponent { get; set; } = typeof(TfCreateIHSpaceStepForm);
    public TfRecipeStepInstance Instance { get; set; }
    public ITfRecipeStepAddonData Data { get; set; }

    public async Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon,
        TfRecipeStepResult stepResult)
    {
        if (addon.GetType().FullName != this.GetType().FullName)
            throw new Exception("Wrong addon type provided for application");

        if (addon.Data.GetType().FullName != typeof(TfCreateIHSpaceStepData).FullName)
            throw new Exception("Wrong data model type provided for application");
        var stepData = (TfCreateIHSpaceStepData)addon.Data;

        ITfService tfService = serviceProvider.GetService<ITfService>()!;
        ITfMetaService tfMetaService = serviceProvider.GetService<ITfMetaService>()!;
        ITalkService talkService = serviceProvider.GetService<ITalkService>()!;
        IAssetsService assetsService = serviceProvider.GetService<IAssetsService>()!;
        var excelDataProvider = tfMetaService.GetDataProviderType(new Guid("7be5a3cd-c922-4e20-99d5-5555f141133c"));
        if (excelDataProvider is null)
            throw new Exception("Excel Data provider not found");

        var roles = tfService.GetRoles();
        var adminRole = roles.Single(x => x.Id == new Guid("3a0c26c5-bd28-4cca-aaf7-5d225b4c3136"));
        var ihRole = roles.Single(x => x.Id == new Guid("39451bfb-997e-492f-8483-41dadd590f24"));
        var defaultTalkChannel = talkService.GetChannels().Single(x => x.Name == "default");
        var defaultAssetsFolder = assetsService.GetFolders().Single(x => x.Name == "default");
        var dataProviders = tfService.GetDataProviders();
        var stepsProvider = dataProviders.Single(x => x.Id == new Guid("cba9f436-1b82-42fb-8e3e-6213d667fb73"));
        var statusProvider = dataProviders.Single(x => x.Id == new Guid("437c9cb1-7f76-4a05-a9b3-c90a714d9768"));
        TfDataTable ihSteps = tfService.QueryDataset(datasetId: stepsProvider.Id); //the default one
        TfDataTable ihStatuses = tfService.QueryDataset(datasetId: statusProvider.Id); //the default one


        #region << Definitions >>

        Guid providerId = Guid.NewGuid();
        Guid spaceId = Guid.NewGuid();
        Guid emailPageId = Guid.NewGuid();
        TfDataProvider provider;
        TfSpace space;
        Dictionary<Guid, Guid> stepDatasetDict = new();
        Dictionary<Guid, Guid> stepPageDict = new();
        Dictionary<Guid, Guid> stepViewDict = new();

        #endregion

        #region <<Create Data provider>>

        {
            var submit = new TfCreateDataProvider
            {
                Id = providerId,
                Name = $"{stepData.BuildingCode}-IH001",
                Index = -1,
                ProviderType = excelDataProvider,
                SettingsJson = JsonSerializer.Serialize(new ExcelDataProviderSettings()),
                SynchPrimaryKeyColumns = [],
                SynchScheduleEnabled = false,
                AutoInitialize = false
            };
            provider = tfService.CreateDataProvider(submit);

            //Update primary key
            var updateSubmit = new TfUpdateDataProvider
            {
                Id = providerId,
                Name = provider.Name,
                SettingsJson = provider.SettingsJson,
                SynchScheduleEnabled = provider.SynchScheduleEnabled,
                SynchScheduleMinutes = provider.SynchScheduleMinutes,
                SynchPrimaryKeyColumns =
                [
                    $"{provider.ColumnPrefix}index", $"{provider.ColumnPrefix}building",
                    $"{provider.ColumnPrefix}manufacturer"
                ]
            };
            provider = tfService.UpdateDataProvider(updateSubmit);

            //delete default dataset
            tfService.DeleteDataset(providerId);
        }

        #endregion

        #region <<Create Data provider Columns>>

        {
            #region << activity >>

            {
                var submit = new TfUpsertDataProviderColumn
                {
                    Id = Guid.NewGuid(),
                    DataProviderId = providerId,
                    SourceName = "Activity",
                    SourceType = "TEXT",
                    CreatedOn = DateTime.Now,
                    DbName = $"{provider.ColumnPrefix}activity",
                    DbType = TfDatabaseColumnType.ShortText,
                    DefaultValue = "N/A",
                    RuleSet = TfDataProviderColumnRuleSet.NotNullableWithCustomDefault,
                    IncludeInTableSearch = true,
                    Expression = null,
                    ExpressionJson = null
                };
                _ = tfService.CreateDataProviderColumn(submit);
            }

            #endregion

            #region << building >>

            {
                var submit = new TfUpsertDataProviderColumn
                {
                    Id = Guid.NewGuid(),
                    DataProviderId = providerId,
                    SourceName = "Building",
                    SourceType = "TEXT",
                    CreatedOn = DateTime.Now,
                    DbName = $"{provider.ColumnPrefix}building",
                    DbType = TfDatabaseColumnType.ShortText,
                    DefaultValue = null,
                    RuleSet = TfDataProviderColumnRuleSet.Nullable,
                    IncludeInTableSearch = true,
                    Expression = null,
                    ExpressionJson = null
                };
                _ = tfService.CreateDataProviderColumn(submit);
            }

            #endregion

            #region << group >>

            {
                var submit = new TfUpsertDataProviderColumn
                {
                    Id = Guid.NewGuid(),
                    DataProviderId = providerId,
                    SourceName = "Group",
                    SourceType = "TEXT",
                    CreatedOn = DateTime.Now,
                    DbName = $"{provider.ColumnPrefix}group",
                    DbType = TfDatabaseColumnType.ShortText,
                    DefaultValue = "N/A",
                    RuleSet = TfDataProviderColumnRuleSet.NotNullableWithCustomDefault,
                    IncludeInTableSearch = true,
                    Expression = null,
                    ExpressionJson = null
                };
                _ = tfService.CreateDataProviderColumn(submit);
            }

            #endregion

            #region << index >>

            {
                var submit = new TfUpsertDataProviderColumn
                {
                    Id = Guid.NewGuid(),
                    DataProviderId = providerId,
                    SourceName = "Index",
                    SourceType = "TEXT",
                    CreatedOn = DateTime.Now,
                    DbName = $"{provider.ColumnPrefix}index",
                    DbType = TfDatabaseColumnType.ShortText,
                    DefaultValue = "N/A",
                    RuleSet = TfDataProviderColumnRuleSet.NotNullableWithCustomDefault,
                    IncludeInTableSearch = true,
                    Expression = null,
                    ExpressionJson = null
                };
                _ = tfService.CreateDataProviderColumn(submit);
            }

            #endregion

            #region << manufacturer >>

            {
                var submit = new TfUpsertDataProviderColumn
                {
                    Id = Guid.NewGuid(),
                    DataProviderId = providerId,
                    SourceName = "Manufacturer",
                    SourceType = "TEXT",
                    CreatedOn = DateTime.Now,
                    DbName = $"{provider.ColumnPrefix}manufacturer",
                    DbType = TfDatabaseColumnType.ShortText,
                    DefaultValue = "N/A",
                    RuleSet = TfDataProviderColumnRuleSet.NotNullableWithCustomDefault,
                    IncludeInTableSearch = true,
                    Expression = null,
                    ExpressionJson = null
                };
                _ = tfService.CreateDataProviderColumn(submit);
            }

            #endregion

            #region << q_ty >>

            {
                var submit = new TfUpsertDataProviderColumn
                {
                    Id = Guid.NewGuid(),
                    DataProviderId = providerId,
                    SourceName = "Q-ty",
                    SourceType = "NUMBER",
                    CreatedOn = DateTime.Now,
                    DbName = $"{provider.ColumnPrefix}q_ty",
                    DbType = TfDatabaseColumnType.Number,
                    DefaultValue = "N/A",
                    RuleSet = TfDataProviderColumnRuleSet.Nullable,
                    IncludeInTableSearch = false,
                    Expression = null,
                    ExpressionJson = null
                };
                _ = tfService.CreateDataProviderColumn(submit);
            }

            #endregion

            #region << rate >>

            {
                var submit = new TfUpsertDataProviderColumn
                {
                    Id = Guid.NewGuid(),
                    DataProviderId = providerId,
                    SourceName = "Rate",
                    SourceType = "NUMBER",
                    CreatedOn = DateTime.Now,
                    DbName = $"{provider.ColumnPrefix}rate",
                    DbType = TfDatabaseColumnType.Number,
                    DefaultValue = "N/A",
                    RuleSet = TfDataProviderColumnRuleSet.Nullable,
                    IncludeInTableSearch = false,
                    Expression = null,
                    ExpressionJson = null
                };
                _ = tfService.CreateDataProviderColumn(submit);
            }

            #endregion

            #region << subgroup >>

            {
                var submit = new TfUpsertDataProviderColumn
                {
                    Id = Guid.NewGuid(),
                    DataProviderId = providerId,
                    SourceName = "Subgroup",
                    SourceType = "TEXT",
                    CreatedOn = DateTime.Now,
                    DbName = $"{provider.ColumnPrefix}subgroup",
                    DbType = TfDatabaseColumnType.ShortText,
                    DefaultValue = "N/A",
                    RuleSet = TfDataProviderColumnRuleSet.NotNullableWithCustomDefault,
                    IncludeInTableSearch = true,
                    Expression = null,
                    ExpressionJson = null
                };
                _ = tfService.CreateDataProviderColumn(submit);
            }

            #endregion

            #region << total >>

            {
                var submit = new TfUpsertDataProviderColumn
                {
                    Id = Guid.NewGuid(),
                    DataProviderId = providerId,
                    SourceName = null,
                    SourceType = null,
                    CreatedOn = DateTime.Now,
                    DbName = $"{provider.ColumnPrefix}total",
                    DbType = TfDatabaseColumnType.Number,
                    DefaultValue = "N/A",
                    RuleSet = TfDataProviderColumnRuleSet.Nullable,
                    IncludeInTableSearch = false,
                    Expression = $"{provider.ColumnPrefix}q_ty * {provider.ColumnPrefix}rate",
                    ExpressionJson = null
                };
                _ = tfService.CreateDataProviderColumn(submit);
            }

            #endregion

            #region << unit >>

            {
                var submit = new TfUpsertDataProviderColumn
                {
                    Id = Guid.NewGuid(),
                    DataProviderId = providerId,
                    SourceName = "Unit",
                    SourceType = "TEXT",
                    CreatedOn = DateTime.Now,
                    DbName = $"{provider.ColumnPrefix}unit",
                    DbType = TfDatabaseColumnType.ShortText,
                    DefaultValue = "N/A",
                    RuleSet = TfDataProviderColumnRuleSet.NotNullableWithCustomDefault,
                    IncludeInTableSearch = true,
                    Expression = null,
                    ExpressionJson = null
                };
                _ = tfService.CreateDataProviderColumn(submit);
            }

            #endregion
        }

        #endregion

        #region <<Create Provider indentities>>

        {
            #region <<ih001_id>>

            {
                var submit = new TfDataProviderIdentity
                {
                    Id = Guid.NewGuid(),
                    DataProviderId = providerId,
                    DataIdentity = "ih001_id",
                    Columns =
                    [
                        $"{provider.ColumnPrefix}index", $"{provider.ColumnPrefix}building",
                        $"{provider.ColumnPrefix}manufacturer"
                    ]
                };
                provider = tfService.CreateDataProviderIdentity(submit);
            }

            #endregion

            #region <<company_name>>

            {
                var submit = new TfDataProviderIdentity
                {
                    Id = Guid.NewGuid(),
                    DataProviderId = providerId,
                    DataIdentity = "company_name",
                    Columns = [$"{provider.ColumnPrefix}manufacturer"]
                };
                provider = tfService.CreateDataProviderIdentity(submit);
            }

            #endregion
        }

        #endregion

        #region <<Create Datasets>>

        {
            foreach (TfDataRow row in ihSteps.Rows)
            {
                var ihStep = GetStepData(row);
                stepDatasetDict[ihStep.Id] = Guid.NewGuid();
                var submit = GetDatasetSubmit(
                    datasetId: stepDatasetDict[ihStep.Id],
                    providerId: providerId,
                    providerPrefix: provider.ColumnPrefix,
                    buildingCode: stepData.BuildingCode,
                    ihStep: ihStep
                );
                _ = tfService.CreateDataset(submit);
            }
        }

        #endregion

        #region <<Create space>>

        {
            #region <<IH001-STP1>>

            {
                var submit = new TfSpace
                {
                    Id = spaceId,
                    Name = $"{stepData.BuildingCode}-IH001",
                    Position = -1,
                    IsPrivate = true,
                    FluentIconName = "BuildingSwap",
                    Color = TfColor.Emerald500,
                    Roles = [ihRole]
                };
                space = tfService.CreateSpace(submit);
            }

            #endregion
        }

        #endregion

        #region <<Create pages >>

        {
            #region << Step pages >>

            {
                short position = 1;
                foreach (TfDataRow row in ihSteps.Rows)
                {
                    var ihStep = GetStepData(row);
                    stepPageDict[ihStep.Id] = Guid.NewGuid();
                    stepViewDict[ihStep.Id] = Guid.NewGuid();
                    var submit = new TfSpacePage()
                    {
                        Id = stepPageDict[ihStep.Id],
                        Name = ihStep.Label,
                        Position = position++,
                        FluentIconName = ihStep.Icon,
                        ParentPage = null,
                        ParentId = null,
                        Type = TfSpacePageType.Page,
                        Description = $"#ih001,#{stepData.BuildingCode}",
                        ChildPages = new(),
                        SpaceId = spaceId,
                        TemplateId = null,
                        ComponentId = new Guid(TucSpaceViewSpacePageAddon.Id),
                        ComponentOptionsJson = JsonSerializer.Serialize(new TfSpaceViewSpacePageAddonOptions
                        {
                            DatasetId = stepDatasetDict[ihStep.Id],
                            SpaceViewId = stepViewDict[ihStep.Id],
                            SpaceViewTemplateId = null
                        })
                    };
                    var (pageId, pages) = tfService.CreateSpacePage(submit);
                    var pageSpaceView = tfService.GetSpaceView(stepViewDict[ihStep.Id]);
                    if (pageSpaceView is null) throw new Exception("Space view was not created");

                    //Delete default columns
                    var spaceViewColumns = tfService.GetSpaceViewColumnsList(stepViewDict[ihStep.Id]);
                    foreach (var column in spaceViewColumns)
                    {
                        tfService.DeleteSpaceViewColumn(column.Id);
                    }
                }
            }

            #endregion

            #region <<Изпратени имейлове>>

            {
                var submit = new TfSpacePage()
                {
                    Id = emailPageId,
                    Name = $"Изпратени имейлове",
                    Position = 9,
                    FluentIconName = "FolderMail",
                    ParentPage = null,
                    ParentId = null,
                    Type = TfSpacePageType.Page,
                    Description = $"#ih001,#{stepData.BuildingCode}",
                    ChildPages = new(),
                    SpaceId = spaceId,
                    TemplateId = null,
                    ComponentId = new Guid(EmailSenderLogSpacePageComponent.Id),
                    ComponentOptionsJson = JsonSerializer.Serialize(new EmailSenderLogSpacePageComponentOptions
                    {
                        RelatedIdValueType = EmailSenderLogRelatedIdValueType.SpaceId,
                        CustomIdValue = null
                    })
                };
                var (pageId, pages) = tfService.CreateSpacePage(submit);
            }

            #endregion
        }

        #endregion

        #region << Fix SpaceViews Settings >>

        {
            foreach (TfDataRow row in ihSteps.Rows)
            {
                var ihStep = GetStepData(row);

                var submit = GetSpaceViewSettings(ihRole.Id, adminRole.Id);
                await tfService.UpdateSpaceViewSettings(stepViewDict[ihStep.Id], submit);
            }
        }

        #endregion

        #region << Add SpaceViewColumns >>

        {
            foreach (TfDataRow row in ihSteps.Rows)
            {
                var ihStep = GetStepData(row);

                var columns = GetSpaceViewColumns(stepViewDict[ihStep.Id], defaultTalkChannel.Id, 
                    defaultAssetsFolder.Id,
                    provider.ColumnPrefix, ihStep);
                foreach (var column in columns)
                {
                    _ = tfService.CreateSpaceViewColumn(column);
                }
            }            
        }

        #endregion

        #region << Add SpaceViewPresets >>

        {
            foreach (TfDataRow stepRow in ihSteps.Rows)
            {
                var ihStep = GetStepData(stepRow);
                var stepStatuses = new List<TfIhStatusData>();
                foreach (TfDataRow statusRow in ihStatuses.Rows)
                {
                    var ihStatus = GetStatusData(statusRow);
                    if(!ihStatus.Code.StartsWith($"{ihStep.Code}-")) continue;
                    stepStatuses.Add(ihStatus);
                }
                var spaceView = tfService.GetSpaceView(stepViewDict[ihStep.Id])!;
                AddSpaceViewPresets(spaceView, ihStep, stepStatuses);
                _ = tfService.UpdateSpaceView(spaceView);                
            }
        }

        #endregion
    }

    public Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult? stepResult)
    {
        return Task.CompletedTask;
    }

    public TfIhStepData GetStepData(TfDataRow row)
    {
        var result = new TfIhStepData();
        result.Id = row.GetRowId();
        result.Code = (string)row[$"dp2_id"]!;
        result.Label = (string)row["dp2_name"]!;
        result.Icon = (string?)row["dp2_icon"]!;

        return result;
    }
    
    public TfIhStatusData GetStatusData(TfDataRow row)
    {
        var result = new TfIhStatusData();
        result.Id = row.GetRowId();
        result.Code = (string)row["dp3_id"]!;
        result.Label = (string)row["dp3_name"]!;
        return result;
    }    

    private TfCreateDataset GetDatasetSubmit(Guid datasetId, Guid providerId, string providerPrefix,
        string buildingCode, TfIhStepData ihStep)
    {
        TfCreateDataset submit = new()
        {
            Id = datasetId,
            Name = $"{buildingCode}-{ihStep.Code}",
            DataProviderId = providerId,
            Columns = ["*"],
        };
        //filters
        if (ihStep.Code == "IH001-STP1")
        {
            submit.Filters = new List<TfFilterBase>()
            {
                new TfFilterOr(
                    new TfFilterText()
                    {
                        Id = Guid.NewGuid(),
                        ColumnName = $"ih001_id.sc_ih001_step",
                        ComparisonMethod = TfFilterTextComparisonMethod.Equal,
                        Value = ihStep.Code,
                    },
                    new TfFilterText()
                    {
                        Id = Guid.NewGuid(),
                        ColumnName = $"ih001_id.sc_ih001_step",
                        ComparisonMethod = TfFilterTextComparisonMethod.HasNoValue,
                        Value = null,
                    },
                    new TfFilterText()
                    {
                        Id = Guid.NewGuid(),
                        ColumnName = $"ih001_id.sc_ih001_step",
                        ComparisonMethod = TfFilterTextComparisonMethod.Equal,
                        Value = String.Empty,
                    }
                )
            };
        }
        else
        {
            submit.Filters = new List<TfFilterBase>()
            {
                new TfFilterText()
                {
                    Id = Guid.NewGuid(),
                    ColumnName = $"ih001_id.sc_ih001_step",
                    ComparisonMethod = TfFilterTextComparisonMethod.Equal,
                    Value = ihStep.Code,
                }
            };
        }

        //sorts
        submit.SortOrders = new List<TfSort>()
        {
            new TfSort()
            {
                ColumnName = $"{providerPrefix}index",
                Direction = TfSortDirection.ASC
            },
            new TfSort()
            {
                ColumnName = $"{providerPrefix}building",
                Direction = TfSortDirection.ASC
            },
            new TfSort()
            {
                ColumnName = $"{providerPrefix}manufacturer",
                Direction = TfSortDirection.ASC
            },
        };

        //data identities
        submit.Identities = new List<TfDatasetIdentity>()
        {
            new TfDatasetIdentity()
            {
                Id = Guid.NewGuid(),
                DataIdentity = "company_name",
                DatasetId = datasetId,
                Columns = ["dp4_name", "dp4_email", "dp4_job_title"]
            },
            new TfDatasetIdentity()
            {
                Id = Guid.NewGuid(),
                DataIdentity = "default",
                DatasetId = datasetId,
                Columns = ["sc_default_assets_counter", "sc_default_talk_counter"]
            },
            new TfDatasetIdentity()
            {
                Id = Guid.NewGuid(),
                DataIdentity = "ih001_id",
                DatasetId = datasetId,
                Columns = ["sc_ih001_status", "sc_ih001_step"]
            }
        };


        return submit;
    }

    private TfSpaceViewSettings GetSpaceViewSettings(Guid ihRoleId, Guid adminRoleId)
    {
        return new TfSpaceViewSettings
        {
            CanCreateRoles = [],
            CanDeleteRoles = [],
            CanUpdateRoles = [ihRoleId, adminRoleId],
            ColoringRules = [],
            FitlerType = TfSpaceViewFilterType.GridFilter,
            FreezeFinalNColumns = null,
            FreezeStartingNColumns = null,
        };
    }

    private List<TfSpaceViewColumn> GetSpaceViewColumns(Guid spaceViewId, Guid defaultTalkChannelId,
        Guid defaultAssetsFolderId, string providerPrefix, TfIhStepData ihStep)
    {
        var result = new List<TfSpaceViewColumn>();
        short position = 1;
        #region << talk >>

        {
            result.Add(new TfSpaceViewColumn()
            {
                Id = Guid.NewGuid(),
                Position = position++,
                TypeId = new Guid(TfTalkCommentsCountViewColumnType.ID),
                TypeOptionsJson = JsonSerializer.Serialize(new TfTalkCommentsCountViewColumnTypeSettings
                {
                    ChannelId = defaultTalkChannelId
                }),
                Settings = new TfSpaceViewColumnSettings()
                {
                    Width = 40,
                    FilterPresentation = TfSpaceViewColumnSettingsFilterPresentation.Hidden,
                    DefaultComparisonMethodDescription = null
                },
                DataMapping = new Dictionary<string, string?>() { { "Value", "default.sc_default_talk_counter" } },
                SpaceViewId = spaceViewId,
                QueryName = NavigatorExt.GenerateQueryName(),
                Title = String.Empty,
                Icon = null,
                OnlyIcon = false,
            });
        }

        #endregion

        #region << assets >>

        {
            result.Add(new TfSpaceViewColumn()
            {
                Id = Guid.NewGuid(),
                Position = position++,
                TypeId = new Guid(TfFolderAssetsCountViewColumnType.ID),
                TypeOptionsJson = JsonSerializer.Serialize(new TfFolderAssetsCountViewColumnTypeSettings
                {
                    FolderId = defaultAssetsFolderId
                }),
                Settings = new TfSpaceViewColumnSettings()
                {
                    Width = 40,
                    FilterPresentation = TfSpaceViewColumnSettingsFilterPresentation.Hidden,
                    DefaultComparisonMethodDescription = null
                },
                DataMapping = new Dictionary<string, string?>() { { "Value", "default.sc_default_assets_counter" } },
                SpaceViewId = spaceViewId,
                QueryName = NavigatorExt.GenerateQueryName(),
                Title = String.Empty,
                Icon = null,
                OnlyIcon = false,
            });
        }

        #endregion

        #region << индекс >>

        {
            result.Add(new TfSpaceViewColumn()
            {
                Id = Guid.NewGuid(),
                Title = "индекс",
                Position = position++,
                TypeId = new Guid(TfTextViewColumnType.ID),
                TypeOptionsJson = JsonSerializer.Serialize(new TfTextViewColumnTypeSettings
                {
                    ChangeConfirmationMessage = null,
                }),
                Settings = new TfSpaceViewColumnSettings()
                {
                    Width = 100,
                    FilterPresentation = TfSpaceViewColumnSettingsFilterPresentation.VisibleWithDefault,
                    DefaultComparisonMethodDescription = "contains"
                },
                DataMapping = new Dictionary<string, string?>() { { "Value", $"{providerPrefix}index" } },
                SpaceViewId = spaceViewId,
                QueryName = NavigatorExt.GenerateQueryName(),
                Icon = null,
                OnlyIcon = false,
            });
        }

        #endregion

        #region << група >>

        {
            result.Add(new TfSpaceViewColumn()
            {
                Id = Guid.NewGuid(),
                Title = "група",
                Position = position++,
                TypeId = new Guid(TfTextViewColumnType.ID),
                TypeOptionsJson = JsonSerializer.Serialize(new TfTextViewColumnTypeSettings
                {
                    ChangeConfirmationMessage = null,
                }),
                Settings = new TfSpaceViewColumnSettings()
                {
                    Width = 140,
                    FilterPresentation = TfSpaceViewColumnSettingsFilterPresentation.VisibleWithDefault,
                    DefaultComparisonMethodDescription = "contains"
                },
                DataMapping = new Dictionary<string, string?>() { { "Value", $"{providerPrefix}group" } },
                SpaceViewId = spaceViewId,
                QueryName = NavigatorExt.GenerateQueryName(),
                Icon = null,
                OnlyIcon = false,
            });
        }

        #endregion

        #region << подгрупа >>

        {
            result.Add(new TfSpaceViewColumn()
            {
                Id = Guid.NewGuid(),
                Title = "подгрупа",
                Position = position++,
                TypeId = new Guid(TfTextViewColumnType.ID),
                TypeOptionsJson = JsonSerializer.Serialize(new TfTextViewColumnTypeSettings
                {
                    ChangeConfirmationMessage = null,
                }),
                Settings = new TfSpaceViewColumnSettings()
                {
                    Width = 140,
                    FilterPresentation = TfSpaceViewColumnSettingsFilterPresentation.VisibleWithDefault,
                    DefaultComparisonMethodDescription = "contains"
                },
                DataMapping = new Dictionary<string, string?>() { { "Value", $"{providerPrefix}subgroup" } },
                SpaceViewId = spaceViewId,
                QueryName = NavigatorExt.GenerateQueryName(),
                Icon = null,
                OnlyIcon = false,
            });
        }

        #endregion

        #region << СМР >>

        {
            result.Add(new TfSpaceViewColumn()
            {
                Id = Guid.NewGuid(),
                Title = "СМР",
                Position = position++,
                TypeId = new Guid(TfTextViewColumnType.ID),
                TypeOptionsJson = JsonSerializer.Serialize(new TfTextViewColumnTypeSettings
                {
                    ChangeConfirmationMessage = null,
                }),
                Settings = new TfSpaceViewColumnSettings()
                {
                    Width = 260,
                    FilterPresentation = TfSpaceViewColumnSettingsFilterPresentation.VisibleWithDefault,
                    DefaultComparisonMethodDescription = "contains"
                },
                DataMapping = new Dictionary<string, string?>() { { "Value", $"{providerPrefix}activity" } },
                SpaceViewId = spaceViewId,
                QueryName = NavigatorExt.GenerateQueryName(),
                Icon = null,
                OnlyIcon = false,
            });
        }

        #endregion

        #region << сграда >>

        {
            result.Add(new TfSpaceViewColumn()
            {
                Id = Guid.NewGuid(),
                Title = "сграда",
                Position = position++,
                TypeId = new Guid(TfTextViewColumnType.ID),
                TypeOptionsJson = JsonSerializer.Serialize(new TfTextViewColumnTypeSettings
                {
                    ChangeConfirmationMessage = null,
                }),
                Settings = new TfSpaceViewColumnSettings()
                {
                    Width = 100,
                    FilterPresentation = TfSpaceViewColumnSettingsFilterPresentation.VisibleWithDefault,
                    DefaultComparisonMethodDescription = "equal"
                },
                DataMapping = new Dictionary<string, string?>() { { "Value", $"{providerPrefix}building" } },
                SpaceViewId = spaceViewId,
                QueryName = NavigatorExt.GenerateQueryName(),
                Icon = null,
                OnlyIcon = false,
            });
        }

        #endregion

        #region << к-во >>

        {
            result.Add(new TfSpaceViewColumn()
            {
                Id = Guid.NewGuid(),
                Title = "к-во",
                Position = position++,
                TypeId = new Guid(TfNumberViewColumnType.ID),
                TypeOptionsJson = JsonSerializer.Serialize(new TfNumberViewColumnTypeSettings
                {
                    ChangeConfirmationMessage = null,
                    Format = "€ ###.##"
                }),
                Settings = new TfSpaceViewColumnSettings()
                {
                    Width = 140,
                    FilterPresentation = TfSpaceViewColumnSettingsFilterPresentation.VisibleWithOptions,
                    DefaultComparisonMethodDescription = "greater or equal"
                },
                DataMapping = new Dictionary<string, string?>() { { "Value", $"{providerPrefix}q_ty" } },
                SpaceViewId = spaceViewId,
                QueryName = NavigatorExt.GenerateQueryName(),
                Icon = null,
                OnlyIcon = false,
            });
        }

        #endregion

        #region << мярка >>

        {
            result.Add(new TfSpaceViewColumn()
            {
                Id = Guid.NewGuid(),
                Title = "мярка",
                Position = position++,
                TypeId = new Guid(TfTextViewColumnType.ID),
                TypeOptionsJson = JsonSerializer.Serialize(new TfTextViewColumnTypeSettings
                {
                    ChangeConfirmationMessage = null,
                }),
                Settings = new TfSpaceViewColumnSettings()
                {
                    Width = 100,
                    FilterPresentation = TfSpaceViewColumnSettingsFilterPresentation.VisibleWithDefault,
                    DefaultComparisonMethodDescription = "equal"
                },
                DataMapping = new Dictionary<string, string?>() { { "Value", $"{providerPrefix}unit" } },
                SpaceViewId = spaceViewId,
                QueryName = NavigatorExt.GenerateQueryName(),
                Icon = null,
                OnlyIcon = false,
            });
        }

        #endregion

        #region << ед. цена >>

        {
            result.Add(new TfSpaceViewColumn()
            {
                Id = Guid.NewGuid(),
                Title = "ед. цена",
                Position = position++,
                TypeId = new Guid(TfNumberViewColumnType.ID),
                TypeOptionsJson = JsonSerializer.Serialize(new TfNumberViewColumnTypeSettings
                {
                    ChangeConfirmationMessage = null,
                    Format = "€ ###.##"
                }),
                Settings = new TfSpaceViewColumnSettings()
                {
                    Width = 140,
                    FilterPresentation = TfSpaceViewColumnSettingsFilterPresentation.VisibleWithOptions,
                    DefaultComparisonMethodDescription = "greater or equal"
                },
                DataMapping = new Dictionary<string, string?>() { { "Value", $"{providerPrefix}rate" } },
                SpaceViewId = spaceViewId,
                QueryName = NavigatorExt.GenerateQueryName(),
                Icon = null,
                OnlyIcon = false,
            });
        }

        #endregion

        #region << ОБЩО >>

        {
            result.Add(new TfSpaceViewColumn()
            {
                Id = Guid.NewGuid(),
                Title = "ОБЩО",
                Position = position++,
                TypeId = new Guid(TfNumberViewColumnType.ID),
                TypeOptionsJson = JsonSerializer.Serialize(new TfNumberViewColumnTypeSettings
                {
                    ChangeConfirmationMessage = null,
                    Format = "€ ###.##"
                }),
                Settings = new TfSpaceViewColumnSettings()
                {
                    Width = 140,
                    FilterPresentation = TfSpaceViewColumnSettingsFilterPresentation.VisibleWithOptions,
                    DefaultComparisonMethodDescription = "greater or equal"
                },
                DataMapping = new Dictionary<string, string?>() { { "Value", $"{providerPrefix}total" } },
                SpaceViewId = spaceViewId,
                QueryName = NavigatorExt.GenerateQueryName(),
                Icon = null,
                OnlyIcon = false,
            });
        }

        #endregion

        #region << изпълнител >>

        {
            result.Add(new TfSpaceViewColumn()
            {
                Id = Guid.NewGuid(),
                Title = "изпълнител",
                Position = position++,
                TypeId = new Guid(TfTextViewColumnType.ID),
                TypeOptionsJson = JsonSerializer.Serialize(new TfTextViewColumnTypeSettings
                {
                    ChangeConfirmationMessage = null,
                }),
                Settings = new TfSpaceViewColumnSettings()
                {
                    Width = null,
                    FilterPresentation = TfSpaceViewColumnSettingsFilterPresentation.VisibleWithOptions,
                    DefaultComparisonMethodDescription = "contains"
                },
                DataMapping = new Dictionary<string, string?>() { { "Value", $"{providerPrefix}manufacturer" } },
                SpaceViewId = spaceViewId,
                QueryName = NavigatorExt.GenerateQueryName(),
                Icon = null,
                OnlyIcon = false,
            });
        }

        #endregion

        return result;
    }

    private void AddSpaceViewPresets(TfSpaceView spaceView, TfIhStepData ihStep, List<TfIhStatusData> stepIhStatuses)
    {
        spaceView.Presets.Clear();
        if(stepIhStatuses.Count <= 1) return;
        foreach (var ihStatus in stepIhStatuses)
        {
            spaceView.Presets.Add(new TfSpaceViewPreset()
            {
                Name = ihStatus.Label,
                Filters = new()
                {
                    new TfFilterText()
                    {
                        Value = ihStatus.Code,
                        ColumnName = "ih001_id.sc_ih001_status",
                        ComparisonMethod = TfFilterTextComparisonMethod.Equal,
                        Id = Guid.NewGuid(),
                    }
                },
                Id = Guid.NewGuid(),
                ParentId = null,
                Search = null,
                SortOrders = new(),
                Presets = new(),
                IsGroup = false,
                Color = null,
                Icon = null,
            });            
        }
    }
}

public record TfCreateIHSpaceStepData : ITfRecipeStepAddonData
{
    public string BuildingCode { get; set; }
}

public record TfIhStepData
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Label { get; set; }
    public string Icon { get; set; }
}

public record TfIhStatusData
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Label { get; set; }
}