import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ResumeAnalyzerService } from '../../services/resume-analyzer.service';

@Component({
  selector: 'app-analysis-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './analysis-history.component.html'
})
export class AnalysisHistoryComponent implements OnInit {

  history: any[] = [];
  selectedAnalysisId: number | null = null;

  @Output() analysisSelected = new EventEmitter<number>();

  constructor(private analyzerService: ResumeAnalyzerService) {}

  openMenuId: number | null = null;
  editingId: number | null = null;
  editedCompanyName: string = '';
  
  toggleMenu(id: number) {
    this.openMenuId = this.openMenuId === id ? null : id;
  }
  
  startEditing(item: any) {
    this.openMenuId = null;
    this.editingId = item.id;
    this.editedCompanyName = item.companyName || '';
  }
  
  cancelEditing() {
    this.editingId = null;
    this.editedCompanyName = '';
  }
  
  async saveCompanyName(item: any) {
    if (!this.editedCompanyName.trim()) return;
    
    item.companyName = this.editedCompanyName;
    this.editingId = null;
    this.editedCompanyName = '';
  
    await this.analyzerService.updateCompanyName(item.id, item.companyName);
  }
  
  async deleteAnalysis(id: number) {
    this.openMenuId = null;
    if (confirm('Are you sure you want to delete this analysis?')) {
      await this.analyzerService.deleteAnalysis(id);
      this.history = this.history.filter(item => item.id !== id);
    }
  }  

  async ngOnInit() {
    this.history = await this.analyzerService.getAnalysisHistory();
  }

  selectAnalysis(id: number) {
    this.selectedAnalysisId = id;
    this.analysisSelected.emit(id);
  }
}
