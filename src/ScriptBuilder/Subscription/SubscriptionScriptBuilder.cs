﻿using System.IO;
using System.Text;

namespace NServiceBus.Persistence.Sql.ScriptBuilder
{
    public static class SubscriptionScriptBuilder
    {
        public static void BuildCreateScript(TextWriter writer, BuildSqlDialect sqlDialect)
        {
            writer.Write(ResourceReader.ReadResource(sqlDialect, "Subscription.Create"));
        }

        public static string BuildCreateScript(BuildSqlDialect sqlDialect)
        {
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder))
            {
                BuildCreateScript(stringWriter, sqlDialect);
            }
            return stringBuilder.ToString();
        }

        public static void BuildDropScript(TextWriter writer, BuildSqlDialect sqlDialect)
        {
            writer.Write(ResourceReader.ReadResource(sqlDialect, "Subscription.Drop"));
        }

        public static string BuildDropScript(BuildSqlDialect sqlDialect)
        {
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder))
            {
                BuildDropScript(stringWriter, sqlDialect);
            }
            return stringBuilder.ToString();
        }

    }
}