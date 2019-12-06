////////////////////////////////////////////////////////////////////////////////
//  
// @module Quick Save for Unity3D 
// @author Michael Clayton
// @support clayton.inds+support@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using Newtonsoft.Json;

#if !NETFX_CORE


namespace FastFrameWork.Core.Serialisers
{
    public class JsonSerialiserMono : IJsonSerialiser
    {
        public string Serialise<T>(T value)
        {

            return JsonConvert.SerializeObject(value);
        }

        public T Deserialise<T>(string json)
        {

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
#endif