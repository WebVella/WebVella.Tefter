﻿namespace WebVella.Tefter.Database;

public record DbDateColumn : DbColumnWithAutoDefaultValue
{
    public override DbType Type => DbType.Date;
}
