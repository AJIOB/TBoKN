#pragma once
#include <string>

std::string EncodeMessage(std::string message);
std::string DecodeMessage(std::string message);

std::string MakeErrors(std::string message, int& lastPosOfMadeError);
// returns true, if error were fixed or if fixing non require
bool FindAndRemoveErrorsIfCan(std::string& message);

std::string Divide(std::string dividiend, std::string divisior);

void CheckMessageLength(const std::string& message, const int requiredLength);
void CheckMessage(const std::string& message, const int requiredLength);
int CalculateOnes(const std::string& message);
void LeftShift(std::string& message);
void RightShift(std::string& message);
std::string Add(const std::string& a1, const std::string& a2);

bool CharToBool(const char c);
char BoolToChar(const bool c);

std::string ULongToString(const unsigned long num, const int stringLen);
unsigned long StringToULong(const std::string& num);
