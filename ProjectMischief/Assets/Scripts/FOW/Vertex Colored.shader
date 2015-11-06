Shader "Custom/Vertex Colored" {
 Properties {
	_Color ("Main Color", Color) = (0,0,0,1)
 }
     SubShader {
             Pass {
                     ColorMaterial AmbientAndDiffuse
             }
     } 
 }