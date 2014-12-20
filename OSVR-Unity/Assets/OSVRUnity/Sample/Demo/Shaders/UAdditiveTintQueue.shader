  Shader "Bully!/Unlit/AdditiveColorQueue" {
    Properties {
      
 
      _MainTex ("Texture", 2D) = "Black" {}
      _Tint ("FresnelColor", Color) = (1,1,1)
      _Multi ("Multiplier", Range(0.0,20)) = 0.0


      
    }
    SubShader {
      Tags { "Queue"="geometry-25" "RenderType" = "Opaque" }
      LOD 200
      ZWrite Off
      Lighting Off
      Cull Back
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
      
      };
      
      sampler2D _MainTex;
      float _Multi;
      float3 _Tint;




      
      void surf (Input IN, inout SurfaceOutput o) {
	      half4 maintex = tex2D (_MainTex, IN.uv_MainTex);
      	  float mask =maintex.a;


 
      	  
      
          //o.Albedo = _LightTint;
          

          o.Emission = maintex*_Multi*_Tint;
          //o.Alpha = _Transparency*mask;
          
          
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }