 Shader "Bully!/Unlit/TextureFadeQueue" {
    Properties {
      _MainTex ("Color Map", 2D) = "white" {}
      _FadeTex ("Fade Map", 2D) = "black" {}
      _LightColor ("Color", Color) = (1,1,1)
    }
    SubShader {
      Tags { "RenderType" = "Opaque" "Queue" = "Geometry-50"}
      ZWrite Off
      Lighting Off
	  Fog  {Mode Off} 
      CGPROGRAM
      #pragma surface surf Unlit novertexlights exclude_path:prepass noambient noforwardadd nolightmap nodirlightmap
	  #pragma target 3.0
	  
	  
      half4 LightingUnlit (SurfaceOutput s, half3 viewDir, half atten) {



		 

          half4 c;
          c.rgb = (s.Albedo);
          c.a = s.Alpha;
          return c;
      }

      struct Input {
         
          fixed2 uv_MainTex;
          fixed2 uv_FadeTex;
 
  

      };
      sampler2D _MainTex;
      sampler2D _FadeTex;
      half4 _LightColor;

     

	  
      void surf (Input IN, inout SurfaceOutput o) {
      half4 maintex = tex2D (_MainTex, IN.uv_MainTex);
      half fadetex = tex2D (_FadeTex, IN.uv_FadeTex).r;
      half multitex = tex2D (_FadeTex, IN.uv_FadeTex).g;

      
      
      
      
    
		  o.Albedo = lerp(maintex*multitex,_LightColor,fadetex);
		  
      }
      ENDCG
    }
    Fallback "Diffuse"
  }