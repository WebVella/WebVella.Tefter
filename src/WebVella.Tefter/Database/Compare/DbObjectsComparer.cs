namespace WebVella.Tefter.Database;

internal static class DbObjectsComparer
{
    public static DifferenceCollection Compare(DbTableCollection initialCollection, DbTableCollection modifiedCollection)
    {
        DifferenceCollection differences = new DifferenceCollection();

        if ((initialCollection == null || initialCollection.Count() == 0) &&
            (modifiedCollection == null || modifiedCollection.Count() == 0))
        {
            return differences;
        }

        //all tables are removed
        if (initialCollection != null && (modifiedCollection == null || modifiedCollection.Count() == 0))
        {
            foreach (DbTable table in initialCollection)
            {
                var diffs = Compare(table, null);
                differences.AddRange(diffs);
            }
        }
        //all tables are new
        else if ((initialCollection == null || initialCollection.Count() == 0) && modifiedCollection != null)
        {
            foreach (DbTable table in modifiedCollection)
            {
                var diffs = Compare(null, table);
                differences.AddRange(diffs);
            }
        }
        else
        {
            HashSet<Guid> processedTables = new HashSet<Guid>();
            foreach (DbTable table in initialCollection)
            {
                processedTables.Add(table.Id);
                var modifiedTable = modifiedCollection.Find(table.Id);

                if (modifiedTable != null && modifiedTable.Name != table.Name)
                {
                    differences.Add(new Difference
                    {
                        Type = DifferenceActionType.Error,
                        ObjectType = DifferenceObjectType.Table,
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
            foreach (DbTable table in modifiedCollection)
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

    private static DifferenceCollection Compare(DbTable initialTable, DbTable modifiedTable)
    {
        DifferenceCollection differences = new DifferenceCollection();

        if (initialTable == null && modifiedTable == null)
            return differences;

        //table is removed
        if (initialTable != null && modifiedTable == null)
        {
            //remove table will remove all related columns, indexes and constraints
            //so no need to generate remove for them
            differences.Add(new Difference
            {
                Type = DifferenceActionType.Remove,
                ObjectType = DifferenceObjectType.Table,
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
            differences.Add(new Difference
            {
                Type = DifferenceActionType.Add,
                ObjectType = DifferenceObjectType.Table,
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

    private static DifferenceCollection Compare(string tableName, DbColumn initialColumn, DbColumn modifiedColumn)
    {
        DifferenceCollection differences = new DifferenceCollection();
        if (initialColumn == null && modifiedColumn == null)
            return differences;

        //column is removed
        if (initialColumn != null && modifiedColumn == null)
        {
            differences.Add(new Difference
            {
                Type = DifferenceActionType.Remove,
                TableName = tableName,
                ObjectType = DifferenceObjectType.Column,
                ObjectName = initialColumn.Name,
                Object = initialColumn,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(initialColumn)} '{initialColumn.Name}' will be removed." }.AsReadOnly()
            });
        }
        //column is added
        else if (initialColumn == null && modifiedColumn != null)
        {
            differences.Add(new Difference
            {
                Type = DifferenceActionType.Add,
                TableName = tableName,
                ObjectType = DifferenceObjectType.Column,
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
                differences.Add(new Difference
                {
                    Type = DifferenceActionType.Error,
                    ObjectType = DifferenceObjectType.Column,
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
                differences.Add(new Difference
                {
                    Type = DifferenceActionType.Error,
                    ObjectType = DifferenceObjectType.Column,
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


            List<string> descriptions = new List<string>();
            if (initialColumn.IsNullable != modifiedColumn.IsNullable)
            {
                if (modifiedColumn.IsNullable)
                    descriptions.Add($"{GetDbObjectTypeName(modifiedColumn)} '{modifiedColumn.Name}' will be made NULLABLE");
                else
                    descriptions.Add($"{GetDbObjectTypeName(modifiedColumn)} '{modifiedColumn.Name}' will be made NOT NULLABLE");
            }

            if (initialColumn.DefaultValue != modifiedColumn.DefaultValue)
            {
                if (modifiedColumn.DefaultValue == null)
                    descriptions.Add($"{GetDbObjectTypeName(modifiedColumn)} '{modifiedColumn.Name}' default value be changed to NULL");
                else
                    descriptions.Add($"{GetDbObjectTypeName(modifiedColumn)} '{modifiedColumn.Name}' default value be changed to '{modifiedColumn.DefaultValue}'");
            }

            if (initialColumn.GetType().IsAssignableFrom(typeof(DbColumnWithAutoDefaultValue)))
            {
                var initialAutoDefaultValue = ((DbColumnWithAutoDefaultValue)initialColumn).AutoDefaultValue;
                var modifiedAutoDefaultValue = ((DbColumnWithAutoDefaultValue)modifiedColumn).AutoDefaultValue;
                if (initialAutoDefaultValue != modifiedAutoDefaultValue)
                {
                    if (modifiedAutoDefaultValue)
                        descriptions.Add($"{GetDbObjectTypeName(initialColumn)} '{initialColumn.Name}' automatic generation of default value will be switched ON.");
                    else
                        descriptions.Add($"{GetDbObjectTypeName(initialColumn)} '{initialColumn.Name}' automatic generation of default value will be switched OFF.");
                }
            }

            if (descriptions.Count > 0)
            {
                differences.Add(new Difference
                {
                    Type = DifferenceActionType.Update,
                    TableName = tableName,
                    ObjectType = DifferenceObjectType.Column,
                    Object = modifiedColumn,
                    ObjectName = modifiedColumn.Name,
                    Descriptions = descriptions.AsReadOnly()
                });
            }
        }

        return differences;
    }

    private static DifferenceCollection Compare(string tableName, DbIndex initialIndex, DbIndex modifiedIndex)
    {
        DifferenceCollection differences = new DifferenceCollection();
        if (initialIndex == null && modifiedIndex == null)
            return differences;

        //index is removed
        if (initialIndex != null && modifiedIndex == null)
        {
            differences.Add(new Difference
            {
                Type = DifferenceActionType.Remove,
                TableName = tableName,
                ObjectName = initialIndex.Name,
                ObjectType = DifferenceObjectType.Index,
                Object = initialIndex,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(initialIndex)} '{initialIndex.Name}' will be removed." }.AsReadOnly()
            });
        }
        //index is added
        else if (initialIndex == null && modifiedIndex != null)
        {
            differences.Add(new Difference
            {
                Type = DifferenceActionType.Add,
                TableName = tableName,
                ObjectName = modifiedIndex.Name,
                ObjectType = DifferenceObjectType.Index,
                Object = modifiedIndex,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedIndex)} '{modifiedIndex.Name}' will be added." }.AsReadOnly()
            });
        }
        //both indexes exists
        else
        {
            if (initialIndex.GetType() != modifiedIndex.GetType())
            {
                differences.Add(new Difference
                {
                    Type = DifferenceActionType.Remove,
                    TableName = tableName,
                    ObjectName = initialIndex.Name,
                    ObjectType = DifferenceObjectType.Index,
                    Object = initialIndex,
                    Descriptions = new List<string> { $"{GetDbObjectTypeName(initialIndex)} '{initialIndex.Name}' will be removed because type of the index was changed." }.AsReadOnly()
                });

                differences.Add(new Difference
                {
                    Type = DifferenceActionType.Add,
                    TableName = tableName,
                    ObjectName = modifiedIndex.Name,
                    ObjectType = DifferenceObjectType.Index,
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
                    differences.Add(new Difference
                    {
                        Type = DifferenceActionType.Remove,
                        TableName = tableName,
                        ObjectName = initialIndex.Name,
                        ObjectType = DifferenceObjectType.Index,
                        Object = initialIndex,
                        Descriptions = new List<string> { $"{GetDbObjectTypeName(initialIndex)} '{initialIndex.Name}' will be removed because index columns are changed." }.AsReadOnly()
                    });

                    differences.Add(new Difference
                    {
                        Type = DifferenceActionType.Add,
                        TableName = tableName,
                        ObjectName = modifiedIndex.Name,
                        ObjectType = DifferenceObjectType.Index,
                        Object = modifiedIndex,
                        Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedIndex)} '{modifiedIndex.Name}' will be added." }.AsReadOnly()
                    });
                }
            }
        }

        return differences;
    }

    private static DifferenceCollection Compare(string tableName, DbConstraint initialConstraint, DbConstraint modifiedConstraint)
    {
        DifferenceCollection differences = new DifferenceCollection();
        if (initialConstraint == null && modifiedConstraint == null)
            return differences;

        //constraint is removed
        if (initialConstraint != null && modifiedConstraint == null)
        {
            differences.Add(new Difference
            {
                Type = DifferenceActionType.Remove,
                TableName = tableName,
                ObjectName = initialConstraint.Name,
                ObjectType = DifferenceObjectType.Constraint,
                Object = initialConstraint,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(initialConstraint)} '{initialConstraint.Name}' will be removed." }.AsReadOnly()
            });
        }
        //constraint is added
        else if (initialConstraint == null && modifiedConstraint != null)
        {
            differences.Add(new Difference
            {
                Type = DifferenceActionType.Add,
                TableName = tableName,
                ObjectName = modifiedConstraint.Name,
                ObjectType = DifferenceObjectType.Constraint,
                Object = modifiedConstraint,
                Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedConstraint)} '{modifiedConstraint.Name}' will be added." }.AsReadOnly()
            });
        }
        //both constraints exists
        else
        {
            if (initialConstraint.GetType() != modifiedConstraint.GetType())
            {
                differences.Add(new Difference
                {
                    Type = DifferenceActionType.Remove,
                    TableName = tableName,
                    ObjectName = initialConstraint.Name,
                    ObjectType = DifferenceObjectType.Constraint,
                    Object = initialConstraint,
                    Descriptions = new List<string> { $"{GetDbObjectTypeName(initialConstraint)} '{initialConstraint.Name}' will be removed because type of the constraint was changed." }.AsReadOnly()
                });

                differences.Add(new Difference
                {
                    Type = DifferenceActionType.Add,
                    TableName = tableName,
                    ObjectName = modifiedConstraint.Name,
                    ObjectType = DifferenceObjectType.Constraint,
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
                    differences.Add(new Difference
                    {
                        Type = DifferenceActionType.Remove,
                        TableName = tableName,
                        ObjectName = initialConstraint.Name,
                        ObjectType = DifferenceObjectType.Constraint,
                        Object = initialConstraint,
                        Descriptions = new List<string> { $"{GetDbObjectTypeName(initialConstraint)} '{initialConstraint.Name}' will be removed because constraint columns are changed." }.AsReadOnly()
                    });

                    differences.Add(new Difference
                    {
                        Type = DifferenceActionType.Add,
                        TableName = tableName,
                        ObjectName = modifiedConstraint.Name,
                        ObjectType = DifferenceObjectType.Constraint,
                        Object = modifiedConstraint,
                        Descriptions = new List<string> { $"{GetDbObjectTypeName(modifiedConstraint)} '{modifiedConstraint.Name}' will be added." }.AsReadOnly()
                    });
                }
                else
                {
                    if (initialConstraint.GetType() == typeof(DbForeignKeyConstraint))
                    {
                        var initialFkConstraint = (DbForeignKeyConstraint)initialConstraint;
                        var modifiedFkConstraint = (DbForeignKeyConstraint)modifiedConstraint;

                        if (initialFkConstraint.ForeignTable != modifiedFkConstraint.ForeignTable)
                        {
                            differences.Add(new Difference
                            {
                                Type = DifferenceActionType.Remove,
                                TableName = tableName,
                                ObjectName = initialConstraint.Name,
                                ObjectType = DifferenceObjectType.Constraint,
                                Object = initialConstraint,
                                Descriptions = new List<string> { $"{GetDbObjectTypeName(initialConstraint)} '{initialConstraint.Name}' will be removed because foreign table is changed." }.AsReadOnly()
                            });

                            differences.Add(new Difference
                            {
                                Type = DifferenceActionType.Add,
                                TableName = tableName,
                                ObjectName = modifiedConstraint.Name,
                                ObjectType = DifferenceObjectType.Constraint,
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
                                differences.Add(new Difference
                                {
                                    Type = DifferenceActionType.Remove,
                                    TableName = tableName,
                                    ObjectName = initialConstraint.Name,
                                    ObjectType = DifferenceObjectType.Constraint,
                                    Object = initialConstraint,
                                    Descriptions = new List<string> { $"{GetDbObjectTypeName(initialConstraint)} '{initialConstraint.Name}' will be removed foreign columns is changed." }.AsReadOnly()
                                });

                                differences.Add(new Difference
                                {
                                    Type = DifferenceActionType.Add,
                                    TableName = tableName,
                                    ObjectName = modifiedConstraint.Name,
                                    ObjectType = DifferenceObjectType.Constraint,
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

    private static string GetDbObjectTypeName(DbObject obj)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");

        switch (obj)
        {
            case DbTable: return "table";
            case DbAutoIncrementColumn: return "auto increment column";
            case DbBooleanColumn: return "boolean column";
            case DbDateColumn: return "Date column";
            case DbDateTimeColumn: return "DateTime column";
            case DbGuidColumn: return "Guid column";
            case DbIdColumn: return "Id column";
            case DbNumberColumn: return "number column";
            case DbTextColumn: return "text column";
            case DbForeignKeyConstraint: return "foreign key constraint";
            case DbPrimaryKeyConstraint: return "primary key constraint";
            case DbUniqueKeyConstraint: return "unique key constraint";
            case DbBTreeIndex: return "btree index";
            case DbGinIndex: return "gin index";
            case DbGistIndex: return "gist index";
            case DbHashIndex: return "hash index";

            default:
                throw new DbException($"Not supported object type: {obj.GetType()}");
        }
    }

    //remove duplicates and sort in order for SQL script generation
    private static DifferenceCollection Process(DifferenceCollection differences)
    {
        DifferenceCollection result = new DifferenceCollection();

        //remove duplicated records
        Dictionary<string, Difference> dict = new Dictionary<string, Difference>();
        foreach (var diff in differences)
        {
            string key = $"{diff.Type}{diff.ObjectType}{diff.TableName}{diff.ObjectName}";
            if (!dict.ContainsKey(key))
                dict[key] = diff;
        }

        List<Difference> filteredList = dict.Values.ToList();

        //remove index
        result.AddRange(filteredList
            .Where(x => x.Type == DifferenceActionType.Remove &&
                    x.ObjectType == DifferenceObjectType.Index));

        //remove constraints
        result.AddRange(filteredList
            .Where(x => x.Type == DifferenceActionType.Remove &&
                    x.ObjectType == DifferenceObjectType.Constraint));

        //remove columns
        result.AddRange(filteredList
            .Where(x => x.Type == DifferenceActionType.Remove &&
                    x.ObjectType == DifferenceObjectType.Column));

        //remove tables
        result.AddRange(filteredList
            .Where(x => x.Type == DifferenceActionType.Remove &&
                x.ObjectType == DifferenceObjectType.Table));

        //add tables
        result.AddRange(filteredList
          .Where(x => x.Type == DifferenceActionType.Add &&
                  x.ObjectType == DifferenceObjectType.Table));

        //add columns
        result.AddRange(filteredList
          .Where(x => x.Type == DifferenceActionType.Add &&
                  x.ObjectType == DifferenceObjectType.Column));

        //add indexes
        result.AddRange(filteredList
          .Where(x => x.Type == DifferenceActionType.Add &&
                  x.ObjectType == DifferenceObjectType.Index));

        //add constraints
        result.AddRange(filteredList
          .Where(x => x.Type == DifferenceActionType.Add &&
                  x.ObjectType == DifferenceObjectType.Constraint));

        //update columns
        result.AddRange(filteredList
          .Where(x => x.Type == DifferenceActionType.Update &&
                  x.ObjectType == DifferenceObjectType.Column));

        if (filteredList.Count != result.Count)
            throw new DbException("Process of processing differences result with incorrect count. This should not happen.");
            
        return result;
    }
}
