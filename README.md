# Ayende Interview  

Ayende (founder of **NHibernate** and **RavenDB**) shared a great interview task. Below is a copy of the original blog post:

---

### Interview Questions and Challenges  

Interview questions are always tough to design. On the one hand, you need to create something that is **not trivial** to do. On the other hand, you have a **hard time limit** for a reasonable solution.  

For example:  
- Implementing a **linked list** is something I would expect anyone to do in an interview.  
- Implementing a **binary tree** (including balancing), however, is probably not feasible within an interview time frame.  

---

### Home Tasks vs. Time Constraints  

Interview tasks that candidates can do **at home** are somewhat easier because there aren’t the same time constraints. However:  
- If the task takes **too long** (e.g., a week to complete), candidates will skip the question — and the position entirely.  
- If the task is **too basic** (e.g., “send a binary tree”), candidates will just:  
   - **Google → Copy & Paste → Send**.  
   - This teaches you **nothing** about their abilities.  

> **Note**: Sometimes you *do* learn a lot if a candidate cannot even manage the “copy & paste” solution. In such cases, they pretty much disqualify themselves. But we could assess that more easily with **Fizz Buzz**, after all.  

---

### The Interview Task  

So, I came up with the following question:  

We have the following file (the full data set is **276 MB**) that contains the **entry/exit log** for a parking lot.  

![File Example](https://github.com/drakon660//AyendeInterview/blob/main/image.png?raw=true)  

The file format:  
- The **first value** is the **entry time**.  
- The **second value** is the **exit time**.  
- The **third value** is the **car ID**.  

#### File Details:  
- Encoding: **UTF-8** text file  
- Delimiter: **Space-separated values**  
- Line Endings: **Windows line endings**  

---

## Parameters  

To generate a data file for testing, use the PowerShell script below.  

### Script Overview  
This script generates a random data file that mimics the format described above. It creates lines with:  
1. Random **ISO 8601 timestamps** (entry and exit times).  
2. Random **8-digit car IDs**.  
3. A customizable **file size** in megabytes (MB).  

---

### Script Parameters  

```powershell
param(
    [int]$FileSizeMB = 1, # Desired file size in MB
    [string]$OutputPath = "data.txt" # Output file path
)
```

> **Note**: The repository includes Ayende original solutions along with my own attempts and final version. Enjoy!

