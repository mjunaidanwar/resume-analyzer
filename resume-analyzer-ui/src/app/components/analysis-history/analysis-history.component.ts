import { Component, EventEmitter, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResumeAnalyzerService } from '../../services/resume-analyzer.service';

@Component({
  selector: 'app-analysis-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './analysis-history.component.html'
})
export class AnalysisHistoryComponent implements OnInit {

  history: any[] = [];
  selectedAnalysisId: number | null = null;

  @Output() analysisSelected = new EventEmitter<number>();

  constructor(private analyzerService: ResumeAnalyzerService) {}

  async ngOnInit() {
    this.history = await this.analyzerService.getAnalysisHistory();
  }

  selectAnalysis(id: number) {
    this.selectedAnalysisId = id;
    this.analysisSelected.emit(id);
  }
}
