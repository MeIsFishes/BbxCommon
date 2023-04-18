Shader "Unlit/DisappearShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" { }
        _DisappearOffset("Disappear Offset", Range(-0.5, 0.5)) = 0.5
        _Diffuse("Diffuse", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _DisappearOffset;
            fixed4 _Diffuse;

            v2f vert(appdata v)
            {
                v2f f;
                f.vertex = UnityObjectToClipPos(v.vertex);
                // pass uv value from v to f, and z value for model clip
                f.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                // for vertex.y locates (-0.5, 0.5) in object space, it fixes the value with a offset
                f.uv.z = _DisappearOffset - v.vertex.y;
                f.normal = v.normal;

                return f;
            }

            fixed4 frag(v2f f) : SV_Target
            {
                clip(f.uv.z);
                
                // calculate diffuse lighting
                fixed3 normalDirection = normalize(mul(f.normal, (float3x3)unity_WorldToObject));
                fixed3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 diffuse = max(dot(normalDirection, lightDirection), 0) * _Diffuse.rgb;   // diffuse direction multiply color

                // sample the texture
                fixed3 col = tex2D(_MainTex, f.uv.xy);
                return fixed4((col * diffuse), 1);   // mix origin color with diffuse lighting
            }
            ENDHLSL
        }
    }
}