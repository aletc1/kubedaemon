using System;
using System.Collections.Generic;
using System.Linq;

namespace Kubedaemon.Models.Statuses
{
    public class JobStatus : Status
    {
        public new class Condition : Status.Condition
        {
            public static Condition Pending => new Condition("Pending", null, DateTimeOffset.Now);
            public static Condition PartiallyComplete => new Condition("PartiallyComplete", null, DateTimeOffset.Now);
            public static Condition Complete => new Condition("Complete", null, DateTimeOffset.Now);
            public static Condition Failed => new Condition("Failed", null, DateTimeOffset.Now);

            public Condition(string code, string message, DateTimeOffset timestamp) : base(code, message, timestamp)
            {
                switch (code)
                {
                    case "Complete":
                        Level = ConditionLevel.Success;
                        break;
                    case "Failed":
                        Level = ConditionLevel.Error;
                        break;
                    case "PartiallyComplete":
                        Level = ConditionLevel.Warning;
                        break;
                    default:
                        Level = ConditionLevel.Info;
                        break;
                }
            }
        }

        public DateTimeOffset StartTime { get; private set; }
        public DateTimeOffset? EndTime { get; private set; }
        public bool Complete => Conditions != null && Conditions.Any(c => c.Code == "Complete") && EndTime.HasValue;
        public new IList<Condition> Conditions => base.Conditions.Select(o => (Condition)o).ToList();

        public JobStatus(string name, string ns, Condition condition, DateTimeOffset? creationTime, DateTimeOffset? startTime, DateTimeOffset? endTime) : this(name, ns, new List<Condition> { condition }, creationTime, startTime, endTime)
        {

        }

        public JobStatus(string name, string ns, IList<Condition> conditions, DateTimeOffset? creationTime, DateTimeOffset? startTime, DateTimeOffset? endTime) : base(name, ns, conditions.Select(o => (Status.Condition)o).ToList(), creationTime)
        {
            StartTime = startTime ?? DateTimeOffset.Now;
            EndTime = endTime;
        }
    }
}
