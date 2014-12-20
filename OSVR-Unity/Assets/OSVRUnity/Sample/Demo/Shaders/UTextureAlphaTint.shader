 Shader "Bully!/Unlit/TextureAlphaTint" {
    Properties {
      _MainTex ("Color Map/Alpha Spec", 2D) = "white" {}
      _Tint ("FresnelColor", Color) = (1,1,1)
      _LightTex ("LightMap", 2D) = "gray" {}
	  _Brightness ("Brightness", Range(0.0,2.0)) = 0.0

    }
    SubShader {
      Tags { "RenderType" = "Opaque" "Queue"="Geometry"}
      CGPROGRAM
      #pragma surface surf Unlit halfasview novertexlights exclude_path:prepass noambient noforwardadd nolightmap nodirlightmap
	  #pragma target 3.0
	  
	  
      half4 LightingUnlit (SurfaceOutput s, half3 viewDir, half atten) {



		 

          half4 c;
          c.rgb = (s.Albedo);
          c.a = s.Alpha;
          return c;
      }

      struct Input {
          float2 uv_MainTex;
          //float2 uv2_LightTex;
  

      };
      sampler2D _MainTex;
      //sampler2D _LightTex;
	  //float _Brightness;
	  float3 _Tint;

	  
      void surf (Input IN, inout SurfaceOutput o) {
      float4 maintex = tex2D (_MainTex, IN.uv_MainTex);
      float mask = maintex.a;
      half3 tintmask = lerp(1,_Tint,mask);
     // float4 light = tex2D (_LightTex, IN.uv2_LightTex); 
      //float bright = _Brightness;
    
		  //o.Albedo = maintex*(light+.5);
		  o.Emission = maintex*tintmask;
		 
		
      }
      ENDCG
    }
    Fallback "Diffuse"
  }