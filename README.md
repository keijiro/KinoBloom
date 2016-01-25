Kino/Bloom v2
=============

*Bloom* is a high-quality image effect that adds bloom/veiling glare effect
onto rendered images.

![screenshot](https://41.media.tumblr.com/d65affb0f4a7ca6e14ba5d6dd5628428/tumblr_o1i5rqBaYc1qio469o1_640.png)

![screenshot](https://36.media.tumblr.com/31261e8cf49c637ffcfdb4d86da41506/tumblr_o1i18y41DH1qio469o1_640.png)

![screenshot](https://36.media.tumblr.com/1f31745016c56f1976e9867eb5355b5e/tumblr_o1i18y41DH1qio469o2_640.png)

*Bloom* is part of the *Kino* effect suite. Please see the [GitHub
repositories][kino] for further information about the suite.

[kino]: https://github.com/search?q=kino+user%3Akeijiro&type=Repositories

System Requirements
-------------------

Unity 5.1 or later versions.

Effect Properties
-----------------

- **Exposure** - Controls sensitivity of the effect with a value from 0.0 (less
  sensitive, especially in darker area) to 1.0 (sensitive in full range).

- **Radius** - Controls the extent of veiling effects. The value is not related
  to screen size and can be controlled in a resolution-independent fashion.

- **Intensity** - The blend ratio of the result image.

- **Anti Flicker** - Sometimes the effect introduces strong flickers (flashing
  noise). This option is used to suppress them with a small-kernel median
  filter.

No Backward Compatibility
-------------------------

This version (v2) is not compatible with the previous versions. You can't simply
upgrade the previous implementation or use two different versions in the same
project. Sorry for the inconvenience!

License
-------

Copyright (C) 2015, 2016 Keijiro Takahashi

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
