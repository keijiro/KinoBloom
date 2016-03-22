Kino/Bloom v2
=============

*Bloom* is a high-quality image effect that adds bloom/veiling glare effect
onto rendered images.

![Screenshot][Image]

*Bloom* is part of the *Kino* effect suite. Please see the [GitHub
repositories][Kino] for further information about the suite.

System Requirements
-------------------

Unity 5.1 or later versions.

Effect Properties
-----------------

![Inspector][Inspector]

- **Threshold** - Filters out pixels under this level of brightness. This value
  should be given in the gamma space (as used in the color picker).

- **Soft Knee** - Makes transition between under/over-threshold gradual (0 =
  hard threshold, 1 = soft threshold).

- **Intensity** - Total intensity of the effect.

- **Radius** - Controls the extent of veiling effects. The value is not related
  to screen size and can be controlled in a resolution-independent fashion.

- **High Quality** - Controls the filter quality and the buffer resolution. On
  mobile platforms, it might strongly affect the performance of the effect,
  therefore it’s recommended to be unchecked when running on mobile devices.

- **Anti Flicker** - Sometimes the effect introduces strong flickers (flashing
  noise). This option is used to suppress them with a noise reduction filter.

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

[Kino]: https://github.com/search?q=kino+user%3Akeijiro&type=Repositories
[Image]: https://41.media.tumblr.com/d65affb0f4a7ca6e14ba5d6dd5628428/tumblr_o1i5rqBaYc1qio469o1_640.png
[Inspector]: https://36.media.tumblr.com/96fb2b51ec0c817bc1b1fdbf5aadbbad/tumblr_o4figiJddx1qio469o1_400.png
