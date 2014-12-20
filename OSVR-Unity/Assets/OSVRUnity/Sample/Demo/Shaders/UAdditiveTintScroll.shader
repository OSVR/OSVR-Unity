  Shader "Bully!/Unlit/AdditiveColorScroll" {
    Properties {
      
 
      _MainTex ("Texture", 2D) = "Black" {}
      _MaskTex ("Texture", 2D) = "Black" {}
      _Tint ("FresnelColor", Color) = (1,1,1)
      _Multi ("Multiplier", Range(0.0,20)) = 0.0
      _ScrollSpeedY ("Scroll y", Range(-50,50)) = 0



      
    }
    SubShader {
      Tags { "Queue"="geometry+15" "RenderType" = "Opaque" }
      LOD 200
      ZWrite off
      Lighting Off
      //Cull Back
      ZTest LEqual
      Blend One One 
      Fog { Color (0,0,0,0) }
      CGPROGRAM
      #pragma target 3.0
     #pragma surface surf UnlitReflect  halfasview novertexlights exclude_path:prepass noambient noforwardadd nolightmap nodirlightmap

      half4 LightingUnlitReflect (SurfaceOutput s, half3 lightDir, half3 viewDir) {
		half3 h = normalize (lightDir + viewDir);


          half4 c;
          c.rgb = s.Albedo;
          c.a = s.Alpha;
          return c;
      }
      struct Input {
          float2 uv_MainTex;
          float3 viewDir;
      
      };
      
      sampler2D _MainTex;
      sampler2D _MaskTex;
      half _ScrollSpeedY;
      float _Multi;
      float3 _Tint;




      
      void surf (Input IN, inout SurfaceOutput o) {

      

      float2 scrollUV = IN.uv_MainTex;
      float yscroll=_ScrollSpeedY * _Time;
      scrollUV +=float2(yscroll,0);
      
      
	      half4 maintex = tex2D (_MainTex,IN.uv_MainTex );
	      half masktex = tex2D (_MaskTex, scrollUV).r;
      	  //float3 mask =clamp(((masktex*frescomp)-.3)/ .2,0,1);


 
      	  
      
          //o.Albedo = _LightTint;
          

          o.Emission = maintex*_Multi*_Tint*masktex;
          //o.Alpha = _Transparency*mask;
          
          
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }