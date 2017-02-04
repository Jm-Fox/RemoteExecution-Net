namespace RemoteExecution.TransportLayer.Lidgren.IT.MS
{
	public interface IGreeter
	{
		string Hello(string name);
	}

	class Greeter : IGreeter
	{
		#region IGreeter Members

		public string Hello(string name)
		{
			return string.Format("Hello {0}!", name);
		}

        #endregion
    }
    public interface ISilent
    {
        void Hello(string name);
    }

    class Silent : ISilent
    {
        #region IGreeter Members

        public void Hello(string name)
        {
            
        }

        #endregion
    }
}