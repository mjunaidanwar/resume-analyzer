import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-analysis-result',
  templateUrl: './analysis-result.component.html',
  standalone: true,
  imports: [CommonModule]
})
export class AnalysisResultComponent {
  @Input() result: {
    similarityScore: number;
    matchingSkills: string[];
    missingSkills: string[];
    recommendations: string;
  } = { similarityScore: 0, matchingSkills: [], missingSkills: [], recommendations: '' };
  
  get similarityScorePercent(): number {
    return this.result.similarityScore * 100;
  }
}