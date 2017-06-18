// Decompiled with JetBrains decompiler
// Type: Web_Camera_Video.Properties.Resources
// Assembly: Web Camera Video, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C170AFF-D588-4EBC-A909-729F0BA3A963
// Assembly location: D:\Visual Studio\Web Camera Video\Web Camera Video\bin\Release\Web Camera Video.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Web_Camera_Video.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Web_Camera_Video.Properties.Resources.resourceMan == null)
          Web_Camera_Video.Properties.Resources.resourceMan = new ResourceManager("Web_Camera_Video.Properties.Resources", typeof (Web_Camera_Video.Properties.Resources).Assembly);
        return Web_Camera_Video.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return Web_Camera_Video.Properties.Resources.resourceCulture;
      }
      set
      {
        Web_Camera_Video.Properties.Resources.resourceCulture = value;
      }
    }

    internal static Bitmap Button_Big
    {
      get
      {
        return (Bitmap) Web_Camera_Video.Properties.Resources.ResourceManager.GetObject("Button_Big", Web_Camera_Video.Properties.Resources.resourceCulture);
      }
    }

    internal Resources()
    {
    }
  }
}
