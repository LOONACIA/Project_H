Sci-Fi Shader Pack is a pack of shaders that will help you in creating your game.


Supported Platforms:
    All platforms


Asset contains:
    Displacement
	Dissolve 
	GostDepthFade 
	GostDetails 
	GostStandart 
	GostUnlit 
	Hologram 
	HologramDepthFade 
	HologramDepthFadeDisplacement 
	HologramFresnelColor 
	HologramTriplanar 
	Outline 
	Sci-Fi 
	XRayOutline


!!! You need to unpack the required package for your project's render type. Packages are stored in a folder Sci-FiShader Pack / Assets!!!

To control dynamic shaders use structure: 
    Shader.SetGlobalFloat("__name__", FLOAT);

Or if not global shader:
	this.GetComponent<Renderer>().material.SetFloat("_name_", FLOAT);

	Where __name__:
	    _ShaderDisplacement
        _ShaderHologramDisplacement
        _ShaderDissolve
        _ShaderSciFi


For emission use post process bloom and enable HDR in render settings.
If one object uses several materials, then it is necessary to increase render queue by one for one of the materials.
The shader was created using Amplify Shader Editor(1.7.7).
3D model where taken from https://www.turbosquid.com/3d-models/3d-robot-weapons-model-1299060 by MarchCAT.


Feedback (suggestions, questions, reports or errors):
    SomeOneWhoCaresFeedBack@gmail.com


My social networks:
    https://www.artstation.com/vrdsgsegs
    https://twitter.com/iOneWhoCares


Version 
    1.1
	    * emission map added
        * color data is saved when switching shaders
	1.2
	    * added HDRP shaders (except outline)
		* opacity in opaque shaders (albedo alfa)
    1.3
	    * added URP shaders (except outline)
    1.4
	    * added shaders with not global variables