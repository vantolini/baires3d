namespace b3d{
    [System.Diagnostics.DebuggerDisplay("{name} {Enabled}  {Type}")]
    public abstract class Component : IComponent
    {
        protected string name;
        public int ID;
        protected bool m_loaded;
        public float Distance;
        public bool Visible;
        public bool Enabled;


        public string FilePath;
        public string FileName;
        public ComponentType Type;

        #region props

        public virtual bool Loaded
        {
            get { return m_loaded; }
            set { m_loaded = value; }
        }

        public virtual string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        #endregion

        public abstract void Initialize();
        public abstract void Dispose();

        protected Component()
        {
        }

        protected Component(string name)
        {
            this.name = name;
        }
    }
}