using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using misechko.com.data.EF;

namespace misechko.com.tests
{
    [TestFixture]
    public class MPContextTestsAndRunners
    {
        private MPDataContext _context;

        public MPContextTestsAndRunners()
        {
            _context = new MPDataContext();
        }

        [Test]
        public void RewriteToLowerAllKeys()
        {
            var savedSuccessfully = false;
            foreach (var contentEl in _context.ContentElements.ToList())
            {
                try
                {
                    var newKey = contentEl.ContentKey.ToLower();
                    if (_context.ContentElements.Where(el => el.ContentKey == newKey).ToList().Count > 1)
                    {
                        _context.ContentElements.Remove(contentEl);
                    } else contentEl.ContentKey = newKey;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            try
            {
                _context.SaveChanges();
                savedSuccessfully = true;
            }
            catch (Exception)
            {
                
            }
            
            Assert.IsTrue(savedSuccessfully);
        }
    }
}
