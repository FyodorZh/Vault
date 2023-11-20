using System;
using System.Runtime.InteropServices;
using OrderedSerializer;
using Vault.Repository;

namespace Vault.Scripting
{
    [Guid("B716E624-B7C9-4345-AB92-E85F825BC1FE")]
    public class LockCommand : Command1
    {
        public override string Name => "lock";
        
        private LockCommand() {}
        
        public LockCommand(string scope)
            : base(new CommandOption(scope))
        {}

        public override Result Process(IProcessorContext context)
        {
            string scope = Option.Name;

            LockUnlock_Result result = new LockUnlock_Result();
            
            switch (scope)
            {
                case "all":
                    result.Content = context.Current.ChildrenContent.Lock();
                    result.Name = context.Current.ChildrenNames.Lock();
                    break;
                case "content":
                    result.Content = context.Current.ChildrenContent.Lock();
                    break;
                case "names":
                    result.Name = context.Current.ChildrenNames.Lock();
                    break;
                default:
                    return Fail("Wrong lock command. Allowed: all/names/content");
            }

            return result;
        }
    }
    
    [Guid("9440FE81-EEF1-4956-B949-95406E472D87")]
    public class LockUnlock_Result : OkResult
    {
        private LockUnlockResult? _name;
        private LockUnlockResult? _content;

        public LockUnlockResult? Name
        {
            get => _name;
            set => _name = value;
        }
            
        public LockUnlockResult? Content
        {
            get => _content;
            set => _content = value;
        }

        public LockUnlock_Result()
        {}

        public override void Serialize(IOrderedSerializer serializer)
        {
            bool isNullName = _name == null;
            serializer.Add(ref isNullName);
            if (!isNullName)
            {
                byte b = (byte)_name!.Value;
                serializer.Add(ref b);
                _name = (LockUnlockResult)b;
            }

            bool isNullContent = _content == null;
            serializer.Add(ref isNullContent);
            if (!isNullContent)
            {
                byte b = (byte)_content!.Value;
                serializer.Add(ref b);
                _content = (LockUnlockResult)b;
            }
        }
    }
}