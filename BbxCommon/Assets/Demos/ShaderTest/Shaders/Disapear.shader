Shader "Unlit/DisappearShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" { }
        _DisappearOffset("Disappear Offset", Range(-0.5, 0.5)) = 0.5
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _DisappearOffset;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //输出的uv中xy存储uv值,z存储用于Clip的参数值
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                //因为模型空间中y值范围为(-0.5,0.5)，所以还需要进行偏移以保证裁剪的正确
                o.uv.z = _DisappearOffset - v.vertex.y;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                clip(i.uv.z);
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv.xy);
                return col;
            }
            ENDHLSL
        }
    }
}