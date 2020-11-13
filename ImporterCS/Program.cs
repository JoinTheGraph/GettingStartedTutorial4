using System;
using System.IO;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;

namespace ImporterCS
{
    class Program
    {
        static void Main(string[] args)
        {
            // Connect to the Gremlin server and create the graph traversal source
            var gremlinServer = new GremlinServer("localhost", 8182);
            using var gremlinClient = new GremlinClient(gremlinServer);
            var driverRemoteConnection = new DriverRemoteConnection(gremlinClient);
            GraphTraversalSource g = AnonymousTraversalSource.Traversal().WithRemote(driverRemoteConnection);
            
            // Create an index for the "userId" property ... if you can! The author cound not find a way to create a TinkerGraph index from
            // a C# program.

            string[] lines = File.ReadAllLines(@"C:\Temp\Wiki-Vote.txt");
            foreach (string line in lines)
            {
                if (!line.StartsWith("#"))
                {
                    string[] lineParts = line.Split('\t');
                    string user1Id = lineParts[0];
                    string user2Id = lineParts[1];

                    Vertex user1Vertex = GetOrCreateUserVertex(g, user1Id);
                    Vertex user2Vertex = GetOrCreateUserVertex(g, user2Id);

                    // Add a "votesFor" edge going from user1 to user2.
                    g.AddE("votesFor").From(user1Vertex).To(user2Vertex).Iterate();
                }
            }

            // Print the number of vertices and the number of edges in the database
            long numberOfVertices = g.V().Count().Next();
            long numberOfEdges = g.E().Count().Next();
            Console.Write($"Database contains { numberOfVertices } vertices and { numberOfEdges } edges.");
        }

        static Vertex GetOrCreateUserVertex(GraphTraversalSource g, string userId)
        {
            Vertex userVertex;

            GraphTraversal<Vertex, Vertex> userTraversal = g.V().Has("user", "userId", userId);
            if (userTraversal.HasNext()) {
                userVertex = userTraversal.Next();
            } else {
                userVertex = g.AddV("user").Property("userId", userId).Next();
            }

            return userVertex;
        }
    }
}