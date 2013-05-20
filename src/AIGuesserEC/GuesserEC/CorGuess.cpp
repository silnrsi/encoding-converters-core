// CorGuess.cpp Implementation of Correspondence Guesser
/* 
Correspondence Guesser version 1.1 Apr 2006
(c) SIL International
Developer - Alan Buseman
Not to be used, modified or distributed to others in any form 
without written permission from Alan Buseman. alan_buseman@sil.org
*/

// For documentation see CorGuess.h

#include "stdafx.h"
#include <string.h> // Only generic string functions used

#include "CorGuess.h"

int iCorrespondenceBack( LPTSTR psz1, LPTSTR psz2, int& iStart, int& iEnd1, int& iEnd2 ) // Return length of correspondence from start
	{
	// Find a difference, and return start and end of difference
	int iDiff = 0;
	iStart = 0;
	iEnd1 = 0;
	iEnd2 = 0;
	int iLen1 = (int)_tcslen( psz1 );
	int iLen2 = (int)_tcslen( psz2 );
	int i1 = iLen1 - 1;
	int i2 = iLen2 - 1;
	while ( i1 > 1 && i2 > 1 ) // Look for first difference
		{
		if ( *(psz1+i1) != *(psz2+i2) ) // If mismatch, note difference
			{
			iStart = i1; // Note start of difference
			bool bSucc = false;
			while ( true ) // Look for end of difference
				{
				if ( i1 < iLen1 - 1 || i2 < iLen2 - 1 ) // If not at the very end, check for one char insert or delete // 1.4bp Allow one letter added at end (made it worse)
					{
					if ( *(psz1+i1-1) == *(psz2+i2) 
							&& *(psz1+i1-2) == *(psz2+i2-1) ) // If match at one char diff, note end of difference
						{
						iEnd1 = i1;
						iEnd2 = i2 + 1;
						bSucc = true;
						break;
						}
					else if ( *(psz1+i1) == *(psz2+i2-1) 
							&& *(psz1+i1-1) == *(psz2+i2-2) ) // If match at one char diff, note end of difference
						{
						iEnd1 = i1 + 1;
						iEnd2 = i2;
						bSucc = true;
						break;
						}
					}
				i1--; // Step to prev char
				i2--;
				if ( *(psz1+i1) == *(psz2+i2) // If match, note end of difference
						&& ( *(psz1+i1-1) == *(psz2+i2-1) // Match two letters
							|| i1 < iLen1 - 3 ) ) // Or have length of at least 3
					{
					iEnd1 = i1 + 1;
					iEnd2 = i2 + 1;
					bSucc = true;
					break;
					}
				else if ( i1 <= 1 || i2 <= 1 ) // If different all the way to front, return failure
					return 0;
				}
			if ( bSucc )
				{
				if ( iEnd1 > 0 && iEnd1 == iLen1 - 1 ) // If correspondence only matched one char, match 2 instead
					{
					iEnd1--;
					iEnd2--;
					}
				if ( iLen1 - iEnd1 > 5 ) // If longer than 5, don't do it as a back correspondence
					return 0;
				return 1;
				}
			}
		i1--; // Step to next char
		i2--;
		}
	return 0; // Return no correspondence
	}

Corresp::Corresp()
	{
	pszSrc = NULL;
	pszTar = NULL;
	iNumInstances = 0;
	iNumExceptions = 0;
	pcorNext = NULL;
	}

#pragma warning(disable : 4996) // don't want warnings in VS.Net 2005 about the insecurity of _tcscpy

Corresp::Corresp( LPCTSTR pszSrc1, LPCTSTR pszTar1 ) // Constructor that allocates strings for source and target
	{
    int len = (int)_tcslen( pszSrc1 ) + 1;
	pszSrc = new TCHAR[ len ];
	_tcscpy( pszSrc, pszSrc1 );
    len = (int)_tcslen( pszTar1 ) + 1;
	pszTar = new TCHAR[ len ];
	_tcscpy( pszTar, pszTar1 );
	iNumInstances = 1;
	iNumExceptions = 0;
	pcorNext = NULL;
	}

Corresp::~Corresp()
	{
	if ( pszSrc )
		delete pszSrc;
	if ( pszTar )
		delete pszTar;
	}

CorrespList::CorrespList()
	{
	pcorFirst = NULL;
	pcorLast = NULL;
	iRequiredSuccessPercent = 50;
	}

CorrespList::~CorrespList()
	{
	ClearAll(); // 1.4vyd 
	}

void CorrespList::ClearAll() // 1.4vyd Add ClearAll function
	{
	while ( pcorFirst )
		{
		Corresp* pcor = pcorFirst->pcorNext;
		delete pcorFirst;
		pcorFirst = pcor;
		}
	}

void CorrespList::Add( Corresp* pcorNew ) // Add a new correspondence to the end ofthe list
	{
	if ( !pcorFirst ) // If list was empty, new becomes first
		pcorFirst = pcorNew;
	else // Else, previous last points to new
		pcorLast->pcorNext = pcorNew;
	pcorLast = pcorNew; // New becomes last
	}

void CorrespList::Add( LPCTSTR pszSrc, LPCTSTR pszTar, bool bCount = false ) // Add a new corresponcence to the end of the list
	{
	if ( bCount )
		{
		Corresp* pcorF = pcorFind( pszSrc, pszTar ); // If already in list, increment count
		if ( pcorF )
			{
			pcorF->iNumInstances++;
			return;
			}
		}
	Add( new Corresp( pszSrc, pszTar ) );
	}

Corresp* CorrespList::pcorFind( LPCTSTR pszSrc, LPCTSTR pszTar ) // Find the same pair, return NULL if not found
	{
	for ( Corresp* pcor = pcorFirst; pcor; pcor = pcor->pcorNext )
		{
		if ( !_tcscmp( pszSrc, pcor->pszSrc ) && !_tcscmp( pszTar, pcor->pszTar ) )
			return pcor;
		}
	return NULL;
	}

Corresp* CorrespList::pcorDelete( Corresp* pcor, Corresp* pcorPrev ) // Delete a correspondence from the list, return next after deleted one
	{
	Corresp* pcorNext = pcor->pcorNext;
	if ( !pcorPrev ) // If first in list, then point top at next
		pcorFirst = pcorNext;
	else
		pcorPrev->pcorNext = pcorNext; // else point prev at next
	delete pcor;
	return pcorNext;
	}

Guesser::Guesser()
	{
	iRequiredSuccessPercent = 30; // Init required success percent
	iMaxSuffLen = 5; // Init max suffix length
	iMinSuffExamples = 3; // Minimum number of examples of suffix to be considered
	}

// =========== Start Main Routines
void Guesser::ClearAll() // 1.4vyd Add ClearAll function
	{
	corlstSuff.ClearAll(); // Guessed suffixes
	corlstRoot.ClearAll(); // Guessed roots
	corlstPref.ClearAll(); // Guess prefixes
	corlst.ClearAll(); // Raw correspondences given to guesser
	}

void Guesser::AddCorrespondence( LPCTSTR pszSrc, LPCTSTR pszTar ) // Make a correspondence to the list
	{
	corlst.Add( pszSrc, pszTar );
	}

bool Guesser::bTargetGuess( LPCTSTR pszSrc, LPTSTR* ppszTar ) // Return target guess
	{
	if ( corlstSuff.bIsEmpty() ) // If correspondences have not been calculated, do it now
		{
		Corresp* pcor = NULL;
		Corresp* pcorPrev = NULL;
		int iStart, iEnd1, iEnd2 = 0;
		for ( pcor = corlst.pcorFirst; pcor; pcor = pcor->pcorNext ) // Make and store all suffix correspondences
			{
			LPTSTR pszS = pcor->pszSrc;
			LPTSTR pszT = pcor->pszTar;
			int iCorrBack = iCorrespondenceBack( pszS, pszT, iStart, iEnd1, iEnd2 ); // See how different guess is from correct
			if ( iCorrBack ) // If there is a correspondence, store it
				{
				pszS += iEnd1;
				pszT += iEnd2;
				corlstSuff.Add( pszS, pszT, true ); // Add to list, count if already there
				}
			}
		pcor = corlstSuff.pcorFirst;
		while ( pcor )
			{
			bool bDelete = false;
			if ( pcor->iNumInstances <= 1 ) // Delete all correspondences that occur only once
				bDelete = true;
			if ( bDelete )				
				pcor = corlstSuff.pcorDelete( pcor, pcorPrev );
			else
				{
				pcorPrev = pcor;
				pcor = pcor->pcorNext;
				}
			}
		for ( Corresp* pcorSuff = corlstSuff.pcorFirst; pcorSuff; pcorSuff = pcorSuff->pcorNext ) // Count exceptions for each correspondence
			{
			LPTSTR pszSuffSrc = pcorSuff->pszSrc;
			LPTSTR pszSuffTar = pcorSuff->pszTar;
			int iLenSrc = (int)_tcslen( pszSuffSrc );
			int iLenTar = (int)_tcslen( pszSuffTar );
			for ( pcor = corlst.pcorFirst; pcor; pcor = pcor->pcorNext ) // Look at each knowledge base pair to see if it is an exception
				{
				LPTSTR pszEndSrc = pcor->pszSrc; // Get end of source of kb pair
				pszEndSrc += _tcslen( pszEndSrc ) - iLenSrc;
				if ( !_tcscmp( pszSuffSrc, pszEndSrc ) ) // If source matches, see if target matches, if not this is an exception
					{
					LPTSTR pszEndTar = pcor->pszTar; // Get end of target of kb pair
					pszEndTar += _tcslen( pszEndTar ) - iLenSrc;
					if ( _tcscmp( pszSuffTar, pszEndTar ) ) // If exception, count it
						pcorSuff->iNumExceptions++;
					}
				}
			}

		pcorPrev = NULL;
		pcor = corlstSuff.pcorFirst; // Delete correspondences that have less than required success percentage
		while ( pcor )
			{
			int iSuccessPercent = ( pcor->iNumInstances * 100 ) / ( pcor->iNumInstances + pcor->iNumExceptions );
			if ( iSuccessPercent < iRequiredSuccessPercent ) // 1.4bd Make required success ratio a parameter
				pcor = corlstSuff.pcorDelete( pcor, pcorPrev );
			else
				{
				pcorPrev = pcor;
				pcor = pcor->pcorNext;
				}
			}
		}
	for ( Corresp* pcorSuff = corlstSuff.pcorFirst; pcorSuff; pcorSuff = pcorSuff->pcorNext ) // See if a guess is possible for this string
		{
		**ppszTar = '\0'; // 1.4vyd Clear target if no guess, safer in case caller tries to read it
		LPTSTR pszSuffSrc = pcorSuff->pszSrc;
		int iLenSrc = (int)_tcslen( pszSuffSrc );
		LPCTSTR pszEndSrc = pszSrc; // Get end of source for guess
		pszEndSrc += _tcslen( pszEndSrc ) - iLenSrc;
		if ( !_tcscmp( pszSuffSrc, pszEndSrc ) ) // If source matches, replace it with target
			{
			if ( _tcslen( pszSrc ) >= ( MAX_GUESS_LENGTH - 10 ) ) // If not enough room for possible guess, don't try
				return false;
			LPTSTR pszSuffTar = pcorSuff->pszTar;
			int iLenTar = (int)_tcslen( pszSuffTar );
			_tcscpy( *ppszTar, pszSrc ); // Copy source to target
			LPTSTR pszStartReplace = *ppszTar + _tcslen( pszSrc ) - iLenSrc;
			_tcscpy( pszStartReplace, pszSuffTar ); // Overwrite end of source with target
			return true;
			}
		}
	return false;
	}
