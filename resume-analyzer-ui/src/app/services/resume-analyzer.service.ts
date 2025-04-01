import { Injectable } from '@angular/core';
import axios from 'axios';

@Injectable({
  providedIn: 'root'
})
export class ResumeAnalyzerService {

  apiUrl = 'http://localhost:5267/api/ResumeAnalyzer'; // Your backend URL

  constructor() { }

  async analyzeResume(resumeFile: File, jobDescription: string) {
    const formData = new FormData();
    formData.append('resumeFile', resumeFile);
    formData.append('jobDescription', jobDescription);

    const response = await axios.post(`${this.apiUrl}/analyze`, formData);
    return response.data;
  }
  async getAnalysisHistory() {
    const response = await axios.get(`${this.apiUrl}/history`);
    return response.data;
  }
  
  async getAnalysisById(id: number) {
    const response = await axios.get(`${this.apiUrl}/history/${id}`);
    return response.data;
  }
}
