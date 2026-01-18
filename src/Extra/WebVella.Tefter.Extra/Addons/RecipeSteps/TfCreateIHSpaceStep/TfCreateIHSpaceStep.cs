using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using WebVella.Tefter.Database;
using WebVella.Tefter.DataProviders.Excel;

namespace WebVella.Tefter.Extra.Addons;

public class TfCreateIHSpaceStep : ITfRecipeStepAddon
{
    //addon
    public Guid AddonId { get; init; } = new Guid("AB2BD6A4-BA92-4D55-A18F-BF962F1EBF02");
    public string AddonName { get; init; } = "Create IH Space";
    public string AddonDescription { get; init; } = "creates a IH Process space based on specifications";
    public string AddonFluentIconName { get; init; } = "PuzzlePiece";
    public Type FormComponent { get; set; } = typeof(TfCreateIHSpaceStepForm);
    public TfRecipeStepInstance Instance { get; set; }
    public ITfRecipeStepAddonData Data { get; set; }

    public Task ApplyStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult stepResult)
    {
        if (addon.GetType().FullName != this.GetType().FullName)
            throw new Exception("Wrong addon type provided for application");

        if (addon.Data.GetType().FullName != typeof(TfCreateIHSpaceStepData).FullName)
            throw new Exception("Wrong data model type provided for application");
        var stepData = (TfCreateIHSpaceStepData)addon.Data;

        ITfService tfService = serviceProvider.GetService<ITfService>()!;
        ITfMetaService tfMetaService = serviceProvider.GetService<ITfMetaService>()!;
        var excelDataProvider = tfMetaService.GetDataProviderType(new Guid("7be5a3cd-c922-4e20-99d5-5555f141133c"));
        if (excelDataProvider is null)
            throw new Exception("Excel Data provider not found");
        Guid providerId = Guid.NewGuid();

        Guid datasetStep1Id = Guid.NewGuid();
        Guid datasetStep2Id = Guid.NewGuid();
        Guid datasetStep3Id = Guid.NewGuid();
        Guid datasetStep4Id = Guid.NewGuid();
        Guid datasetStep5Id = Guid.NewGuid();
        Guid datasetStep6Id = Guid.NewGuid();
        Guid datasetStep61Id = Guid.NewGuid();
        Guid datasetStep7Id = Guid.NewGuid();

        Guid spaceId = Guid.NewGuid();

        TfDataProvider provider;
        TfDataset datasetStep1;
        TfDataset datasetStep2;
        TfDataset datasetStep3;
        TfDataset datasetStep4;
        TfDataset datasetStep5;
        TfDataset datasetStep6;
        TfDataset datasetStep61;
        TfDataset datasetStep7;


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
            #region <<IH001-STP1>>
            {
                var submit = GetDatasetSubmit(
                    datasetId: datasetStep1Id,
                    providerId: providerId,
                    providerPrefix: provider.ColumnPrefix,
                    buildingCode: stepData.BuildingCode,
                    stepValue: "IH001-STP1"
                );
                datasetStep1 = tfService.CreateDataset(submit);
            }
            #endregion

            #region <<IH001-STP2>>
            {
                var submit = GetDatasetSubmit(
                    datasetId: datasetStep2Id,
                    providerId: providerId,
                    providerPrefix: provider.ColumnPrefix,
                    buildingCode: stepData.BuildingCode,
                    stepValue: "IH001-STP2"
                );
                datasetStep2 = tfService.CreateDataset(submit);
            }
            #endregion
            
            #region <<IH001-STP3>>
            {
                var submit = GetDatasetSubmit(
                    datasetId: datasetStep3Id,
                    providerId: providerId,
                    providerPrefix: provider.ColumnPrefix,
                    buildingCode: stepData.BuildingCode,
                    stepValue: "IH001-STP3"
                );
                datasetStep3 = tfService.CreateDataset(submit);
            }
            #endregion          
            
            #region <<IH001-STP4>>
            {
                var submit = GetDatasetSubmit(
                    datasetId: datasetStep4Id,
                    providerId: providerId,
                    providerPrefix: provider.ColumnPrefix,
                    buildingCode: stepData.BuildingCode,
                    stepValue: "IH001-STP4"
                );
                datasetStep4 = tfService.CreateDataset(submit);
            }
            #endregion               
            #region <<IH001-STP5>>
            {
                var submit = GetDatasetSubmit(
                    datasetId: datasetStep5Id,
                    providerId: providerId,
                    providerPrefix: provider.ColumnPrefix,
                    buildingCode: stepData.BuildingCode,
                    stepValue: "IH001-STP5"
                );
                datasetStep5 = tfService.CreateDataset(submit);
            }
            #endregion             
            #region <<IH001-STP6>>
            {
                var submit = GetDatasetSubmit(
                    datasetId: datasetStep6Id,
                    providerId: providerId,
                    providerPrefix: provider.ColumnPrefix,
                    buildingCode: stepData.BuildingCode,
                    stepValue: "IH001-STP6"
                );
                datasetStep6 = tfService.CreateDataset(submit);
            }
            #endregion       
            #region <<IH001-STP61>>
            {
                var submit = GetDatasetSubmit(
                    datasetId: datasetStep61Id,
                    providerId: providerId,
                    providerPrefix: provider.ColumnPrefix,
                    buildingCode: stepData.BuildingCode,
                    stepValue: "IH001-STP61"
                );
                datasetStep61 = tfService.CreateDataset(submit);
            }
            #endregion            
            #region <<IH001-STP7>>
            {
                var submit = GetDatasetSubmit(
                    datasetId: datasetStep7Id,
                    providerId: providerId,
                    providerPrefix: provider.ColumnPrefix,
                    buildingCode: stepData.BuildingCode,
                    stepValue: "IH001-STP7"
                );
                datasetStep7 = tfService.CreateDataset(submit);
            }
            #endregion               
        }

        #endregion

        #region <<Create space>>

        {
        }

        #endregion

        #region <<Create pages >>

        {
        }

        #endregion

        return Task.CompletedTask;
    }

    public Task ReverseStep(IServiceProvider serviceProvider, ITfRecipeStepAddon addon, TfRecipeStepResult? stepResult)
    {
        return Task.CompletedTask;
    }

    private TfCreateDataset GetDatasetSubmit(Guid datasetId, Guid providerId, string providerPrefix,
        string buildingCode, string stepValue)
    {
        TfCreateDataset submit = new()
        {
            Id = datasetId,
            Name = $"{buildingCode}-{stepValue}",
            DataProviderId = providerId,
            Columns = ["*"],
        };
        //filters
        if (stepValue == "IH001-STP1")
        {
            submit.Filters = new List<TfFilterBase>()
            {
                new TfFilterOr(
                    new TfFilterText()
                    {
                        Id = Guid.NewGuid(),
                        ColumnName = $"ih001_id.sc_ih001_step",
                        ComparisonMethod = TfFilterTextComparisonMethod.Equal,
                        Value = stepValue,
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
                    Value = stepValue,
                }
            };
        }

        //sorts
        submit.SortOrders = new List<TfSort>()
        {
            new TfSort()
            {
                ColumnName = $"{providerPrefix}_index",
                Direction = TfSortDirection.ASC
            },
            new TfSort()
            {
                ColumnName = $"{providerPrefix}_building",
                Direction = TfSortDirection.ASC
            },
            new TfSort()
            {
                ColumnName = $"{providerPrefix}_manufacturer",
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
}

public record TfCreateIHSpaceStepData : ITfRecipeStepAddonData
{
    public string BuildingCode { get; set; }
}