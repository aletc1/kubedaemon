using System;
using System.Collections.Generic;
using System.Linq;

namespace Kubedaemon.Models.Statuses
{
    public class AppStatus : Status
    {
        public new class Condition : Status.Condition
        {
            public static Condition Pending => new Condition("Pending", null, DateTimeOffset.Now);
            public static Condition Available => new Condition("Available", null, DateTimeOffset.Now);
            public static Condition Failed => new Condition("Failed", null, DateTimeOffset.Now);

            public Condition(string code, string message, DateTimeOffset timestamp) : base(code, message, timestamp)
            {
                switch (code)
                {
                    case "Available":
                        Level = ConditionLevel.Success;
                        break;
                    case "Failed":
                    case "ReplicaFailure":
                        Level = ConditionLevel.Error;
                        break;
                    default:
                        Level = ConditionLevel.Info;
                        break;
                }
            }
        }
        public bool Available => Conditions != null && Conditions.Any(c => c.Code == "Available");
        public new IList<Condition> Conditions => base.Conditions.Select(o => (Condition)o).ToList();

        public AppStatus(string name, string ns, Condition condition, DateTimeOffset? creationTime) : this(name, ns, new List<Condition> { condition }, creationTime)
        {

        }

        public AppStatus(string name, string ns, IList<Condition> conditions, DateTimeOffset? creationTime) : base(name, ns, conditions.Select(o => (Status.Condition)o).ToList(), creationTime)
        {

        }
    }
}
