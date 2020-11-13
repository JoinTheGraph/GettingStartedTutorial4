def getOrCreateUserVertex(g, userId) { 
    def userVertex;

    userTraversal = g.V().has('user', 'userId', userId);
    if (userTraversal.hasNext()) {
        userVertex = userTraversal.next();
    } else {
        userVertex = g.addV('user').property('userId', userId).next();
    }

    return userVertex;
}

// Create an empty graph
graph = TinkerGraph.open();
// Create an index for the "userId" property
graph.createIndex('userId', Vertex.class);
// Create the graph traversal source
g = graph.traversal();

lines = new File('C:\\Temp\\Wiki-Vote.txt').readLines();
for (line in lines) {
    if (!line.startsWith("#")) {
        lineParts = line.split('\t');
        user1Id = lineParts[0];
        user2Id = lineParts[1];

        user1Vertex = getOrCreateUserVertex(g, user1Id);
        user2Vertex = getOrCreateUserVertex(g, user2Id);
        
        // Add a "votesFor" edge going from user1 to user2.
        g.addE('votesFor').from(user1Vertex).to(user2Vertex).iterate();
    }
}

// Print the number of vertices and the number of edges in the database
numberOfVertices = g.V().count().next();
numberOfEdges = g.E().count().next();
println 'Database contains ' + numberOfVertices + ' vertices and ' + numberOfEdges + ' edges.';