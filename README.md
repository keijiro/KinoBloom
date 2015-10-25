KinoBloom
=========

*Bloom* is an optimized implementation of a bloom effect.

![screenshot](https://40.media.tumblr.com/a131dcab21381d25c5e28651043928f0/tumblr_nws635zSaH1qio469o1_1280.png)

*Bloom* has some special features:

- Two different types of filter kernels: fringe blur (small gaussian filter)
  and diffusion blur (large box filter).
- Temporal low-pass filtering: rejects brightly oscillating pixels to eliminate
  unpleasant flicker.

System Requirements
-------------------

*Bloom* is optimized for HDR and linear rendering. Altough it can run without
these features, it's strongly recommended to enable them for optimal quality.

Because of this requirement, it's not recommended to use on mobile platforms
that lack linear rendering.

License
-------

Copyright (C) 2015 Keijiro Takahashi

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
