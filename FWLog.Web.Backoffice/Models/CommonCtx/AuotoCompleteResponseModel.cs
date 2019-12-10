using Newtonsoft.Json;
using System.Collections.Generic;

namespace FWLog.Web.Backoffice.Models.CommonCtx
{
    public class AutoCompleteResponseModel
    {
        [JsonProperty("suggestions")]
        public IEnumerable<AutoCompleteSuggestionModel> Suggestions { get; set; }

        public AutoCompleteResponseModel()
        {

        }

        /// <summary>
        /// Retorna um resultado para autocomplete.
        /// </summary>
        /// <param name="suggestions">As sugestões que apare~cerão no autocomplete.</param>
        public AutoCompleteResponseModel(IEnumerable<AutoCompleteSuggestionModel> suggestions)
        {
            this.Suggestions = suggestions;
        }
    }

    public class AutoCompleteSuggestionModel
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        public AutoCompleteSuggestionModel()
        {

        }

        /// <summary>
        /// Retorna um resultado de suggestion utilizado para autocomplete.
        /// </summary>
        /// <param name="value">O texto que é exibido no suggestion.</param>
        /// <param name="data">Um objeto que contém o identificador do suggestion. Esse valor não é exibido.</param>
        public AutoCompleteSuggestionModel(string value, object data)
        {
            this.Value = value;
            this.Data = data;
        }
    }
}