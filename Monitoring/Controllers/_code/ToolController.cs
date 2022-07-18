using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Controllers
{
    class _ToolController : BaseController
    {
        public object Index()
        {
            string content;
            using (var sr = new System.IO.StreamReader(Vst.GUI.GUI.TemplateFolder + "model.txt"))
            {
                content = sr.ReadToEnd();
                sr.Close();
            }
            return View(content);
        }
    }
}
