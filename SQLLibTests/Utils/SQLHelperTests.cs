using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLib.Utils.Tests
{
    [TestClass()]
    public class SQLHelperTests
    {
        [TestMethod()]
        public void TestConnectionTest()
        {
            var connectedConnString = "Data Source=118.129.159.133;Initial Catalog=IPodStock;Persist Security Info=True;User ID=ipodstockuser;Password=ipodstock12*";
            var disconnectedConnString = "Data Source=118.129.159.133;Initial Catalog=IPodStock;Persist Security Info=True;User ID=ipodstockuser;Password=ipodstock123*";

            Assert.IsTrue(SQLHelper.TestConnection(connectedConnString));
            Assert.IsFalse(SQLHelper.TestConnection(disconnectedConnString));
        }
    }
}