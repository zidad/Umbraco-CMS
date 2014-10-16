﻿using System.Linq;
using Umbraco.Core.Configuration;
using Umbraco.Core.Persistence.DatabaseModelDefinitions;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Umbraco.Core.Persistence.Migrations.Upgrades.TargetVersionSix
{
    [Migration("6.0.0", 11, GlobalSettings.UmbracoMigrationName)]
    public class UpdateCmsContentTable : MigrationBase
    {
        public override void Up()
        {
            //Some very old schemas don't have an index on the cmsContent.nodeId column, I'm not actually sure when it was added but 
            // it is absolutely required to exist in order to add other foreign keys and much better for perf, so we'll need to check it's existence
            // this came to light from this issue: http://issues.umbraco.org/issue/U4-4133
            var dbIndexes = SqlSyntaxContext.SqlSyntaxProvider.GetDefinedIndexes(Context.Database)
                .Select(x => new DbIndexDefinition()
                {
                    TableName = x.Item1,
                    IndexName = x.Item2,
                    ColumnName = x.Item3,
                    IsUnique = x.Item4
                }).ToArray();
            if (dbIndexes.Any(x => x.IndexName.InvariantEquals("IX_cmsContent")) == false)
            {
                Create.Index("IX_cmsContent").OnTable("cmsContent").OnColumn("nodeId").Ascending().WithOptions().Unique();
            }
        }

        public override void Down()
        {
        }
    }
}