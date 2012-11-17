using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using AE.Net.Mail;

namespace Tests
{
  [TestClass]
  public class MyTest
  {
    [TestMethod]
    public void TestMethod1()
    {
      Pop3Client pc = new Pop3Client("mail.jacobgudbjerg.dk", "foto@jacobgudbjerg.dk", "jacobgud");

      var pi = pc.GetMessageCount();

//      var mes = pc.GetMessage(0);

      //mes.Attachments.First().Save(@"c:\temp\foto.jpg");

      pc.Dispose();

      ImapClient IC = new ImapClient("imap.gmail.com", "jacobgudbjerg@gmail.com", "", ImapClient.AuthMethods.Login,993,true);
      var i = IC.GetMessageCount("Inbox");

      var mes = IC.GetMessage(IC.GetMessageCount()-1);

      mes.Attachments.First().Save(@"c:\temp\foto.jpg");
      IC.Dispose();

    }
  }
}
