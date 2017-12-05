using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Using text mesh pro edition
/// </summary>
public class DialogueWriter : MonoBehaviour {

    // The text you actually see on 
    // screen and in the speech bubble UI
    [SerializeField]
    private TMPro.TextMeshPro dialogueTextMeshPro;

    private string  _textToWrite;
    private int     _writerIndex;
    private bool    _doneWriting;
    private float   _delayTilNextCharacter;
    private float   _timeWaitedTilNextCharacter;
    private float   _customAdditionalDelay; // AdditionalDelay only affects the text delay in FixedUpdate
                                            // Make initial delay bigger if you want to add more wait on the first character of new text
    public bool DoneWriting{ get{ return _doneWriting; } }
    public float _baseTextDelay;

    void Start ()
    {
        _timeWaitedTilNextCharacter = 0.0f;
        _customAdditionalDelay      = 0.0f;
        _writerIndex = 0;
        _doneWriting = true;
    }

    public void SetNewText(string newTextToWrite, float initialDelay = 1.0f, float customAdditonalDelay = 0.0f)
    {
        _textToWrite = newTextToWrite;
        _delayTilNextCharacter = initialDelay;
        _customAdditionalDelay = customAdditonalDelay;

        _timeWaitedTilNextCharacter = 0.0f;
        _writerIndex = 0;
        _doneWriting = (newTextToWrite == string.Empty); // Only set done writing to false if the new text isn't empty.

        ClearCurrentText();
    }

    public void FinishWritingEarly()
    {
        if(!_doneWriting)
            _writerIndex = _textToWrite.Length - 1;
    }
	
	void FixedUpdate ()
    {
        if (!_doneWriting)
        {
            _timeWaitedTilNextCharacter += Time.fixedDeltaTime;

            if (_timeWaitedTilNextCharacter > _delayTilNextCharacter)
            {
                _timeWaitedTilNextCharacter = 0.0f;
                _delayTilNextCharacter = 0.05f + _customAdditionalDelay;

                // Checks for punctations or spaces to increase the time or not.
                // We don't want the talk sound to play when it's not a letter or number.
                switch (_textToWrite[_writerIndex])
                {
                    case ' ':
                        _delayTilNextCharacter = 0.035f;
                        break;
                    case ',':
                        _delayTilNextCharacter += 0.20f;
                        break;
                    case '!':
                        _delayTilNextCharacter += 0.4f;
                        break;
                    case '?':
                        _delayTilNextCharacter += 0.4f;
                        break;
                    case '.':
                        _delayTilNextCharacter += 0.5f;
                        break;
                    default:
                        SoundManager._instance.PlaySound("Narrate");
                        break;
                }

                dialogueTextMeshPro.text = _textToWrite.Substring(0, _writerIndex+1);

                _writerIndex++;
                if (_writerIndex >= _textToWrite.Length)
                    _doneWriting = true;
            }
        }
    }

    private void ClearCurrentText()
    {
        dialogueTextMeshPro.text = "";
    }
}
