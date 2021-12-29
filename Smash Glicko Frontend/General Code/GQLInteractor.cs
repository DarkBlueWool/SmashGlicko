using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Smash_Glicko_Frontend.Models;
using System.Diagnostics;

namespace Smash_Glicko_Frontend.Shortcuts
{
    public class GQLInteractor
    {
        static GraphQLHttpClient Client = new GraphQLHttpClient("https://api.smash.gg/gql/alpha", new NewtonsoftJsonSerializer());


        public static async Task<EventModel> GetEventData(string EventSlug, string ApiToken)
        {
            EventModel Output = new EventModel(EventSlug);

            Client.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ApiToken);

            GraphQLHttpRequest Request = new GraphQLHttpRequest
            {
                Query =
                        @"query EventSets($eventSlug: String!, $page: Int!, $perPage: Int!) {
                          event(slug: $eventSlug) {
                            name
                            sets(
                              page: $page
                              perPage: $perPage
                              sortType: STANDARD
                            ) {
                              pageInfo {
                                total
                              }
                              nodes {
                                displayScore(mainEntrantId: 1)
                                slots {
                                  entrant {
                                    id
                                  }
                                }
                              }
                            }
                          }
                        }",
                Variables = new
                {
                    eventSlug = EventSlug,
                    page = 1,
                    perPage = 900
                }
            };

            GraphQL.GraphQLResponse<Models.GraphQL.EventGet.EventResponseType> Response = await Client.SendQueryAsync<Models.GraphQL.EventGet.EventResponseType>(Request);

            //If error, return blank for now. I'll properly parse or report errors later
            if (Response.Errors.Length > 0) return Output;

            Output.EventSlug = EventSlug;
            HashSet<uint> PlayerIDs = new HashSet<uint>();
            foreach(Models.GraphQL.EventGet.NodeType Node in Response.Data.Event.Sets.Nodes)
            {
                if (Node.DisplayScore != "DQ")
                {
                    Output.Player1Wins.Add(ushort.Parse(Node.DisplayScore.Split("-", StringSplitOptions.TrimEntries)[0]));
                    Output.Player2Wins.Add(ushort.Parse(Node.DisplayScore.Split("-", StringSplitOptions.TrimEntries)[1]));
                    Output.Player1ID.Add(uint.Parse(Node.Slots[0].Entrant.Id));
                    Output.Player2ID.Add(uint.Parse(Node.Slots[1].Entrant.Id));
                    PlayerIDs.Add(uint.Parse(Node.Slots[0].Entrant.Id));
                    PlayerIDs.Add(uint.Parse(Node.Slots[1].Entrant.Id));
                }
            }

            Output.PlayerCount = (uint)PlayerIDs.Count;

            return Output;
        }
    }
}
