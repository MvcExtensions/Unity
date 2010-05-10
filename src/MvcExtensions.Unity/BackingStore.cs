#region Copyright
// Copyright (c) 2009 - 2010, Kazi Manzur Rashid <kazimanzurrashid@gmail.com>.
// This source is subject to the Microsoft Public License. 
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL. 
// All other rights reserved.
#endregion

namespace MvcExtensions.Unity
{
    using System.Collections;
    using System.Threading;
    using System.Web;

    internal static class BackingStore
    {
        private static readonly string Key = typeof(BackingStore).FullName;

        public static IDictionary Get()
        {
            IDictionary backingStore = (HttpContext.Current != null) ? HttpContext.Current.Items : null;

            if (HttpContext.Current == null)
            {
                backingStore = Thread.GetData(Thread.GetNamedDataSlot(Key)) as IDictionary;

                if (backingStore == null)
                {
                    backingStore = new Hashtable();
                    Thread.SetData(Thread.GetNamedDataSlot(Key), backingStore);
                }
            }

            return backingStore;
        }
    }
}