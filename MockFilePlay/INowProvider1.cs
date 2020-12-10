using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockFilePlay
{
    public interface INowProvider
    {
        DateTime GetNow();
    }
}
