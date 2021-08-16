using System;
using System.Collections.Generic;
using System.Linq;
using EZNEW.Development.Command;
using EZNEW.Development.Command.Modification;
using EZNEW.Development.Query;

namespace EZNEW.Development.UnitOfWork
{
    public class EmptyActivationRecord : IActivationRecord
    {
        private EmptyActivationRecord(){ }

        public int Id { get; set; }
        public IActivationRecord ParentRecord { get; set; }
        public ActivationOperation Operation { get; set; }
        public string IdentityValue { get; set; }
        public string RecordIdentity { get { return string.Empty; } }
        public IQuery Query { get; set; }
        public IModification ModifyExpression { get; set; }
        public bool IsObsolete => true;
        public void AddFollowRecords(params IActivationRecord[] records)
        {
            IEnumerable<IActivationRecord> collection = records;
            AddFollowRecords(collection);
        }
        public void AddFollowRecords(IEnumerable<IActivationRecord> records)
        {
            throw new InvalidOperationException($"{nameof(EmptyActivationRecord)} cannot perform this operation");
        }
        public ICommand GetExecuteCommand()
        {
            return null;
        }
        public IEnumerable<IActivationRecord> GetFollowRecords()
        {
            return Enumerable.Empty<IActivationRecord>();
        }
        public void Obsolete()
        {
        }
        public readonly static EmptyActivationRecord Default = new EmptyActivationRecord();
    }
}
