import torch
import transformers
import huggingface_hub


# Natural Language Processing models
class NaturalLanguageProcessing:

    # Initialization
    def __init__(self):
        self.device = "cuda" if torch.cuda.is_available() else "cpu"
        print(f"Device: {self.device.upper()}")

    # Text Classification / Sentiment Analysis (https://huggingface.co/distilbert/distilbert-base-uncased-finetuned-sst-2-english)
    def SentimentAnalysis(self):
        pipeline = transformers.pipeline(task="sentiment-analysis")
        print(pipeline(["I am really excited about Machine Learning.", "I am not a big fan of this."]))

    # Zero-Shot Classification (https://huggingface.co/facebook/bart-large-mnli)
    def ZeroShotClassification(self):
        pipeline = transformers.pipeline(task="zero-shot-classification")
        print(pipeline("This is Machine Learning-based application.", candidate_labels=["education", "politics", "business"]))

    # Text Generation / Completion (https://huggingface.co/openai-community/gpt2)
    def TextGeneration(self, model):
        pipeline = transformers.pipeline(task="text-generation", model=model)
        print(pipeline("In this application, I will teach you how to", max_length=300, num_return_sequences=5))

    # Text Generation / Completion (https://huggingface.co/meta-llama/Meta-Llama-3-8B)
    def TextGenerationLlama(self, model):
        huggingface_hub.login("hf_XXXXXXXXXXX")
        pipeline = transformers.pipeline(task="text-generation", model=model, model_kwargs={"torch_dtype": torch.bfloat16}, max_new_tokens=64, device_map="auto")
        print(pipeline("Hey how are you doing today?"))

    #  Mask Filling (https://huggingface.co/distilbert/distilroberta-base)
    def MaskFilling(self, possibilities=2):
        pipeline = transformers.pipeline(task="fill-mask")
        print(pipeline("This application will teach you all about <mask> models.", top_k=possibilities))

    # Token Classification / Named Entity Recognition (https://huggingface.co/dbmdz/bert-large-cased-finetuned-conll03-english)
    def NamedEntityRecognition(self, model):
        pipeline = transformers.pipeline(task="ner", model=model, grouped_entities=True)
        print(pipeline("My name is John and I teach at MetroTech in Brooklyn."))

    # Question Answering (https://huggingface.co/distilbert/distilbert-base-cased-distilled-squad)
    def QuestionAnswering(self):
        pipeline = transformers.pipeline(task="question-answering")
        print(pipeline(question="Where do I work?", context="My name is Sylvain and I work at National Grid in Brooklyn."))

    # Summarization (https://huggingface.co/sshleifer/distilbart-cnn-12-6)
    def Summarization(self, model):
        pipeline = transformers.pipeline(task="summarization", model=model)
        print(
            pipeline(
                """
            America has changed dramatically during recent years. Not only has the number of 
            graduates in traditional engineering disciplines such as mechanical, civil, 
            electrical, chemical, and aeronautical engineering declined, but in most of 
            the premier American universities engineering curricula now concentrate on 
            and encourage largely the study of engineering science. As a result, there 
            are declining offerings in engineering subjects dealing with infrastructure, 
            the environment, and related issues, and greater concentration on high 
            technology subjects, largely supporting increasingly complex scientific 
            developments. While the latter is important, it should not be at the expense 
            of more traditional engineering.

            Rapidly developing economies such as China and India, as well as other 
            industrial countries in Europe and Asia, continue to encourage and advance 
            the teaching of engineering. Both China and India, respectively, graduate 
            six and eight times as many traditional engineers as does the United States. 
            Other industrial countries at minimum maintain their output, while America 
            suffers an increasingly serious decline in the number of engineering graduates 
            and a lack of well-educated engineers.
        """
            )
        )

    # Translation (https://huggingface.co/google-t5/t5-small)
    def Translation(self, model, input, fromLanguage, toLanguage):
        tokenizer = transformers.T5Tokenizer.from_pretrained(model)
        model = transformers.T5ForConditionalGeneration.from_pretrained(model, return_dict=True)
        input_ids = tokenizer(f"translate {fromLanguage} to {toLanguage}: {input}", return_tensors="pt").input_ids
        outputs = model.generate(input_ids)
        decoded = tokenizer.decode(outputs[0], skip_special_tokens=True)
        print(decoded)
