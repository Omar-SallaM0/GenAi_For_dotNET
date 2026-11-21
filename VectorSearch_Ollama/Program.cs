using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OllamaSharp;
using VectorSearch_Ollama;

IEmbeddingGenerator<string, Embedding<float>> generator =
    new OllamaApiClient(new Uri("http://localhost:11434/"), "all-minilm");

// Create the vector store
var vectorStore = new InMemoryVectorStore();
var moviesStore = vectorStore.GetCollection<int, Movie>("movies");

await moviesStore.EnsureCollectionExistsAsync();

foreach (var movie in MovieData.Movies)
{
    movie.Vector = await generator.GenerateVectorAsync(movie.Description);

    await moviesStore.UpsertAsync(movie);
}

// generate the embedding vector for the user's prompt
var query = "I want to see family friendly movie";
//var query = "A science fiction movie about space travel";
var queryEmbedding = await generator.GenerateVectorAsync(query);

// search the knowledge store based on the user's prompt
var searchResults = moviesStore.SearchAsync(queryEmbedding, top: 2);

// see the results just so we know what they look like
await foreach (var result in searchResults)
{
    Console.WriteLine($"Title: {result.Record.Title}");
    Console.WriteLine($"Description: {result.Record.Description}");
    Console.WriteLine($"Score: {result.Score}");
    Console.WriteLine();
}