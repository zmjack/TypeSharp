namespace TypeSharp.Test
{
    public class ChildClass : SuperClass
    {
        public override int OverrideInt { get => base.OverrideInt; set => base.OverrideInt = value; }
        public new NestedClass NewObject { get => base.NewObject as NestedClass; set => base.NewObject = value; }
    }
}
