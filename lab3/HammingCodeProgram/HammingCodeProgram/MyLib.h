#pragma once
#include <deque>

std::deque<bool> EncodeMessage(std::string message);
std::string DecodeMessage(std::deque<bool> message, int& numOfFoundErrors);

std::deque<bool> BytesToBits(std::string bytes);
std::string BitsToBytes(std::deque<bool> bits);
