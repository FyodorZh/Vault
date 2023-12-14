using System;
using System.Runtime.InteropServices;
using OrderedSerializer;
using Vault.Repository;

namespace Vault.Commands
{
    [Guid("B716E624-B7C9-4345-AB92-E85F825BC1FE")]
    public class LockCommand : Command
    {
        private string _scope;
        
        public override string Name => "lock";

        private LockCommand()
        {
            _scope = "";
        }

        public LockCommand(string scope)
        {
            _scope = scope;
        }

        public override Result Process(IProcessorContext context)
        {
            LockUnlock_Result result = new LockUnlock_Result();
            
            switch (_scope)
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

        public override void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _scope, () => throw new Exception());
        }
    }
    
    [Guid("9440FE81-EEF1-4956-B949-95406E472D87")]
    public class LockUnlock_Result : OkResult
    {
        private LockUnlockResult _name;
        private LockUnlockResult _content;

        public LockUnlockResult Name
        {
            get => _name;
            set => _name = value;
        }
            
        public LockUnlockResult Content
        {
            get => _content;
            set => _content = value;
        }
        
        public override void WriteTo(IOutputTextStream dst)
        {
            dst.WriteLine("Name: " + _name);
            dst.WriteLine("Content: " + _content);
        }

        public override void Serialize(IOrderedSerializer serializer)
        {
            serializer.Add(ref _name);
            serializer.Add(ref _content);
        }
    }
}