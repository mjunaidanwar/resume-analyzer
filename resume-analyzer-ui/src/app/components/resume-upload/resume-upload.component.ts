import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AnalysisResultComponent } from '../analysis-result/analysis-result.component';
import { AnalysisHistoryComponent } from '../analysis-history/analysis-history.component';
import { ResumeAnalyzerService } from '../../services/resume-analyzer.service';

@Component({
  selector: 'app-resume-upload',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    AnalysisResultComponent,
    AnalysisHistoryComponent
  ],
  templateUrl: './resume-upload.component.html'
})
export class ResumeUploadComponent {
  resumeFile: File | null = null;
  jobDescription: string = '';
  analysisResult: any = null;
  loading = false;
  selectedFileName: string = '';
  isDragging = false;
  sidebarOpen = true;

  constructor(private analyzerService: ResumeAnalyzerService) { }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.processFile(files[0]);
    }
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    this.processFile(file);
  }

  private processFile(file: File) {
    // File type validation
    const allowedTypes = ['application/pdf', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
    if (!allowedTypes.includes(file.type)) {
      alert('Please upload a PDF or DOCX file');
      return;
    }

    // File size validation (5MB limit)
    if (file.size > 5 * 1024 * 1024) {
      alert('File size should not exceed 5MB');
      return;
    }

    this.resumeFile = file;
    this.selectedFileName = file.name;
  }

  async analyze() {
    if (!this.resumeFile || !this.jobDescription.trim()) {
      alert('Please select a file and enter job description.');
      return;
    }

    try {
      this.loading = true;
      this.analysisResult = null;
      this.analysisResult = await this.analyzerService.analyzeResume(this.resumeFile, this.jobDescription);
    } catch (error) {
      console.error('Analysis error:', error);
      alert('Failed to analyze resume. Please try again.');
    } finally {
      this.loading = false;
    }
  }

  async loadAnalysisFromHistory(id: number) {
    this.loading = true;
    this.analysisResult = await this.analyzerService.getAnalysisById(id);
    this.loading = false;
  }

  // Optional: Method to clear the form
  clearForm() {
    this.resumeFile = null;
    this.selectedFileName = '';
    this.jobDescription = '';
    this.analysisResult = null;
  }
}