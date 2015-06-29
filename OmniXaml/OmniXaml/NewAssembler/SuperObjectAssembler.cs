namespace OmniXaml.NewAssembler
{
    using System;
    using Assembler;

    public class SuperObjectAssembler : IObjectAssembler
    {
        public SuperObjectAssembler(WiringContext wiringContext)
        {
            WiringContext = wiringContext;
            State.Push(new Level());
        }

        public object Result { get; set; }
        public EventHandler<XamlSetValueEventArgs> XamlSetValueHandler { get; set; }
        public WiringContext WiringContext { get; }

        public AssemblerState State { get; } = new AssemblerState();

        public void Process(XamlNode node)
        {
            Command command;

            switch (node.NodeType)
            {
                case XamlNodeType.NamespaceDeclaration:
                    command = new NamespaceDeclarationCommand(this, node.NamespaceDeclaration);
                    break;
                case XamlNodeType.StartObject:
                    command = new StartObjectCommand(this, node.XamlType);
                    break;
                case XamlNodeType.StartMember:
                    command = new StartMemberCommand(this, node.Member);
                    break;
                case XamlNodeType.Value:
                    command = new ValueCommand(this, node.Value);
                    break;
                case XamlNodeType.EndObject:
                    command = new EndObjectCommand(this);
                    break;
                case XamlNodeType.EndMember:
                    command = new EndMemberCommand(this);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            command.Execute();
        }

        public void OverrideInstance(object instance)
        {
        }
    }
}