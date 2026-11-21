# GenAi_dotNET

A hands-on collection of **Generative AI samples in .NET**, using both **local models with [Ollama](https://ollama.com)** (no API key needed) and **cloud models with [OpenAI](https://platform.openai.com)**.

The solution walks through the core building blocks of modern AI apps:

1. **Text completion / chat with LLMs**
2. **Image analysis with vision models**
3. **Embeddings and vector search** (cosine similarity)
4. **Retrieval-Augmented Generation (RAG)** in a full Blazor chat app

---

## Repository Structure

```
GenAi_dotNET.slnx
├── Text_Completion/            # Text completion using OpenAI (console)
├── Text_Completion_Ollama/     # Text completion / chat using local Ollama (console)
├── ImageAnalysis/              # Vision model: analyze images (console)
│   └── images/cars.jpg
├── VectorSearch_Ollama/        # Embeddings + cosine similarity search (console)
│   └── Movie.cs
└── Chat_RAG_Ollama/            # Blazor chat app with RAG over your documents
    ├── Components/             # Layout, Pages, App.razor, Routes.razor
    └── Services/               # Chunking, embeddings, retrieval, chat logic
```

---

## Prerequisites

- [.NET SDK 9.0 or later](https://dotnet.microsoft.com/download) (the `.slnx` solution format requires a recent SDK)
- [Ollama](https://ollama.com/download) installed and running (default endpoint: `http://localhost:11434`), for the Ollama-based samples
- An [OpenAI API key](https://platform.openai.com/api-keys), for the OpenAI-based sample(s)
- Pull the models used by the samples (adjust to the ones configured in the code):

```bash
# Chat / text generation model
ollama pull llama3.2

# Vision model (for ImageAnalysis)
ollama pull llava

# Embedding model (for VectorSearch and RAG)
ollama pull nomic-embed-text
```

### OpenAI API key

Never commit your API key to Git. Provide it through an environment variable or .NET user secrets:

```bash
# Option 1: environment variable
export OPENAI_API_KEY="sk-..."          # Linux / macOS
setx OPENAI_API_KEY "sk-..."            # Windows (restart the terminal afterwards)

# Option 2: .NET user secrets (per project)
cd Text_Completion
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "sk-..."
```

> Check `Program.cs` in each project (and `appsettings.json` in `Chat_RAG_Ollama`) for the exact model names.

---

## Getting Started

```bash
git clone https://github.com/Omar-SallaM0/GenAi_dotNET.git
cd GenAi_dotNET

# Make sure Ollama is running, then run any project:
dotnet run --project Text_Completion_Ollama
dotnet run --project ImageAnalysis
dotnet run --project VectorSearch_Ollama
dotnet run --project Chat_RAG_Ollama
```

---

## Projects

### 1. Text Completion (`Text_Completion`, `Text_Completion_Ollama`)

The "Hello World" of LLMs. Both console apps send a prompt to a language model and print the generated response. The only difference is the provider:

| Project | Provider | Runs where | Needs |
|---|---|---|---|
| `Text_Completion` | **OpenAI** | Cloud | API key |
| `Text_Completion_Ollama` | **Ollama** | Local machine | Ollama running + pulled model |

**Concepts covered:**
- Connecting to OpenAI and to Ollama from .NET
- Sending prompts and receiving completions
- Choosing a model and basic configuration
- Local vs. cloud trade-offs: privacy and cost (local) vs. model quality and no hardware requirements (cloud)

### 2. Image Analysis (`ImageAnalysis`)

Uses a **vision-capable (multimodal) model** to understand images. The sample loads `images/cars.jpg`, sends it to the model along with a text prompt, and prints the model's description of what it sees.

**Concepts covered:**
- Multimodal prompts (text + image)
- Sending image bytes to a vision model
- Asking questions about an image (describe, count objects, identify details)

### 3. Vector Search (`VectorSearch_Ollama`)

Shows how **semantic search** works, finding results by *meaning* rather than exact keywords.

The sample defines a `Movie` model (`Movie.cs`), generates an **embedding** for each movie, and then searches by embedding the user's query and comparing it against the stored vectors.

**How it works:**

1. **Embed the data:** each movie (title, description, etc.) is converted into a vector (an array of floats) by an embedding model.
2. **Embed the query:** the search text is converted into a vector using the same model.
3. **Compare with cosine similarity:** the query vector is compared against every movie vector.
4. **Rank the results:** movies with the highest similarity score are returned first.

### 4. Chat RAG (`Chat_RAG_Ollama`)

A **Blazor web app** that lets you chat with an LLM that answers using *your own documents*, using **Retrieval-Augmented Generation (RAG)**.

**The RAG pipeline:**

```
                 ┌────────────── Ingestion (one time) ──────────────┐
 Documents  ──►  Chunking  ──►  Embedding model  ──►  Vector store
                 └──────────────────────────────────────────────────┘

                 ┌──────────────── Query (every question) ──────────────────┐
 User question ─► Embed question ─► Cosine similarity search ─► Top-K chunks
                                                                    │
                                   Prompt = question + chunks ◄─────┘
                                                │
                                                ▼
                                       LLM generates answer
                 └──────────────────────────────────────────────────────────┘
```

Step by step:

1. **Chunking:** large documents are split into smaller pieces (chunks), optionally with overlap. This keeps each piece focused and small enough to embed and fit into the model's context window.
2. **Embeddings:** each chunk is turned into a vector with an embedding model, so that text with similar meaning ends up with similar vectors.
3. **Vector storage:** the chunks and their vectors are stored so they can be searched quickly.
4. **Retrieval:** the user's question is embedded, and **cosine similarity** is used to find the most relevant chunks (top-K).
5. **Augmentation:** the retrieved chunks are added to the prompt as context.
6. **Generation:** the LLM answers the question grounded in that context, which reduces hallucinations and lets it use information it was never trained on.

---

## Key Concepts

| Concept | Explanation |
|---|---|
| **LLM** | A large language model that generates text from a prompt. |
| **Vision model** | An LLM that can also take images as input. |
| **Embedding** | A numeric vector representing the *meaning* of a piece of text. |
| **Chunking** | Splitting long documents into smaller, overlapping pieces before embedding. |
| **Vector search** | Finding the items whose vectors are closest to a query vector. |
| **Cosine similarity** | A measure of how similar two vectors are, based on the angle between them. |
| **RAG** | Retrieving relevant data and feeding it to an LLM so it answers with up-to-date, private, or domain-specific knowledge. |

### Cosine Similarity

$$
\text{cosine\_similarity}(A, B) = \frac{A \cdot B}{\|A\| \, \|B\|} = \frac{\sum_{i=1}^{n} A_i B_i}{\sqrt{\sum_{i=1}^{n} A_i^2} \; \sqrt{\sum_{i=1}^{n} B_i^2}}
$$

- Result close to **1** → very similar meaning
- Result close to **0** → unrelated
- Result close to **-1** → opposite

---

## Tech Stack

- **.NET** (C#)
- **Blazor** (Razor Components) for the chat UI
- **Ollama** for running LLMs, vision models, and embedding models locally
- **OpenAI API** for cloud-hosted models
- **Microsoft.Extensions.AI** abstractions (if used in your code)

---

## Troubleshooting

- **Connection refused:** make sure Ollama is running (`ollama serve`) and listening on `http://localhost:11434`.
- **Model not found:** run `ollama pull <model-name>` for the model referenced in the code.
- **401 Unauthorized (OpenAI):** the API key is missing or invalid; check your environment variable or user secrets.
- **429 Too Many Requests (OpenAI):** you hit a rate limit or ran out of credits; check your OpenAI billing/usage page.
- **Slow responses:** use a smaller model, or make sure Ollama is using your GPU.

---
 ## 👨‍💻 Author

**Omar Ahmed Sallam**

AI Full-Stack (.NET & Angular) Developer

* GitHub: [Omar-SallaM0](https://github.com/Omar-SallaM0)
* LinkedIn: [Omar Sallam](https://www.linkedin.com/in/omar-sallam0/)
