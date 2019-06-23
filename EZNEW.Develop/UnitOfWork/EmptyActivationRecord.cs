using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;

namespace EZNEW.Develop.UnitOfWork
{
    public class EmptyActivationRecord : IActivationRecord
    {
        private EmptyActivationRecord()
        { }

        public int Id { get; set; }
        public IActivationRecord ParentRecord { get; set; }
        public ActivationOperation Operation { get; set; }
        public string IdentityValue { get; set; }
        public string RecordIdentity { get { return string.Empty; } }
        public IQuery Query { get; set; }
        public IModify ModifyExpression { get; set; }
        public bool IsObsolete => true;

        public void AddFollowRecords(params IActivationRecord[] records)
        {
        }

        public ICommand GetExecuteCommand()
        {
            return null;
        }

        public List<IActivationRecord> GetFollowRecords()
        {
            return new List<IActivationRecord>(0);
        }

        public void Obsolete()
        {
        }

        public readonly static EmptyActivationRecord Default = new EmptyActivationRecord();
    }
}
