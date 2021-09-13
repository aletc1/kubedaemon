using System;
using System.Collections.Generic;

namespace Kubedaemon.Models.Statuses
{
    public class Status
    {
        public class Condition
        {
            public string Code { get; private set; }
            public string Message { get; private set; }
            public DateTimeOffset Timestamp { get; private set; }
            public ConditionLevel Level { get; protected set; }

            public Condition(string code, string message, DateTimeOffset timestamp)
            {
                Code = code;
                Message = message;
                Level = ConditionLevel.Info;
                Timestamp = timestamp;
            }
        }

        public string Name { get; private set; }
        public string Namespace { get; private set; }
        public IList<Condition> Conditions { get; private set; } 
        public DateTimeOffset CreationTime { get; private set; }

        public Status(string name, string ns, Condition condition, DateTimeOffset? creationTime): this(name, ns, new List<Condition> { condition }, creationTime)
        {

        }

        public Status(string name, string ns, IList<Condition> conditions, DateTimeOffset? creationTime)
        {
            Name = name;
            Namespace = ns;
            Conditions = conditions;
            CreationTime = creationTime ?? DateTimeOffset.Now;
        }
    }
}
