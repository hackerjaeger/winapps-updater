﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.10.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "8bc81304ec127d6b605c5766f1dc52bb3eed7dd56f36899407a67f828e6942138c1b1a5056d25765580572ab7f79808f5eacd70c76a28750814e789af605e7fd" },
                { "ar", "116697f2e75003cab19e4078de91a490e96d2c2077084a70b1e180f9b3b0526a51539ccfa8af58ca63a7e9132cc8a4591e40daff7be6c0b6b0f583fc0c55d95e" },
                { "ast", "dba976a4dbf367a226a01fdbb90213a9a0f95ee3792e464fd1b9e622382ead9fb2a25ebc21825fd3b85b297d9d171631ee32c37915e6b1584ad7af8e686b3c31" },
                { "be", "6f15d09e1caf47f9601a2cf16547b54137e68e8df0827a9121de55e290f1ccaf1bd6946d71c6f66b03d3929b0bb59b7d1ad65f41e2ea5c5ce89612296f244d07" },
                { "bg", "1d541af44cc6e83740d5c33fb2d7ed94ec0353cb7ce7838cab1725424ce60d178e01071f9eb139b428c952c0aa6f97be860900b028daa9b80ff0e11a376745d2" },
                { "br", "de493d1c52c6c32c54adb4f85e85c375d4526405fb4cea756134a1df0ebdbf71d8b6f0327d784b46fc22f59e9a8fb23871cf5c610c088b3ea37ccdbb6f9baf02" },
                { "ca", "83bb43a3f9c3612b6fc037b7cbfd2da152b079a3566c035bfcea52fd10733cd270dfeeb5d3888c70ac529a70deb6e7ba20369f2eb8b54c91b27981b98e1e9e0d" },
                { "cak", "9733b2f0e1058ba5b9649009eb5a251f2b23965a972d8373f7bcc872a0e73d368d42fa60a0b3db568872b4ee001c244173fdcc2b7c4048bfe7b41abb97f6b9e8" },
                { "cs", "1d3b41a046577afc2715bf8b68c79e2962d628862bda9018079a16c86ead8f8f5c85a676f1968da8855436565333d2a0dc6cff3c8f4bbeedfda23ec29cbf012e" },
                { "cy", "17bdacdaff73ae7bba5637723d082b725beedb716624a4e112c539ac144fab7bd60873f38156433075178304796afc95e5e7de34feffe47d1cba68ef8f42fe14" },
                { "da", "00898afbe89ba5ebd10ec4733cc599c9b1425ff729bc1f3740f233306a948f285917db0aa8fb5d7bd4d11f1989a01ed5172f9aa08f939cc9875803df38e4c696" },
                { "de", "b6046d2eef0ff99acfc16a691ad44e9dbdf513b370a56653e089c74026496c28b713c2bce530c975594beddd98261a32a29175e03f109bf16dbc5156e0370eeb" },
                { "dsb", "10222d63c611d8caa9e17f68eb64346444bb196c3469e82ab4c4eb719cd091bbec069999633cc386478a1b211e4d2cdc2c9a7aba43928f310f79ac67ff646cd7" },
                { "el", "d3f19a2759a41c27a9fc050132b304b206149b2ee17c994dfc47581271195510e9d2580648570a38589cb12a04d9e1662e21bb6ae63f37991d42a0104495de1a" },
                { "en-CA", "9ceba910e4ec2ea540d328e7a7b7dd730b75b8ba6c8f7f0038ef1f6bef2319b2890fefa9aebccb43f542fa1968c6211e444bb764598d03a39c529feb4a20ade4" },
                { "en-GB", "d9def8b5438a8690c43b642d4af26ba0daa193e7a2dffac3a8879ad81358845339fbd438650904f422432303d3532b6471ff8419b204fc1b8f900551efea111c" },
                { "en-US", "c24448e1afe42748ce6ed47770713821ad0e90d5051a4eb8b872811c57dff6718627d28ee663fb231270b675b025acc0df8406e53cd0d8ac55ac5ffd1353a33f" },
                { "es-AR", "713fa0daf0c53e9b5d05717e601781721343f8697d9962cfe75323a3f7072ce64d2d7cfa45bdd7777f6ab8d6bec49342a801a3dc07c016a38cd9ca0ab4233ace" },
                { "es-ES", "c3b11789c19285e9ffb95bc024560aae446f25fdec0305608245da6fdcb65ae1b2b54819619c9b9637ceb25bdb2e0d60aeda35c2c06849fd6c6ca3ebb988abd0" },
                { "es-MX", "87157902d4e2b1050d865a2a1f39351e4d66c43c190fceab2fafd873691eae45ceb5d53f0842c657619bd59b50d0c04ff32e88cfed05a4d6f2343b50a314323d" },
                { "et", "021340cdde15bd6e37b3e859962160eed7584c728d163c0912c61442d982e3f404ad39aeb6361391ec111e704b593230ad24e0378a132b788cd7962b3b0831db" },
                { "eu", "2e6b09bcd3d147e7d7756318f1b9127512b7d39f6225de2591a3549e578a895b9c6941ac0c72eb870ed00e7d2932dd05abea2b6965d35017111143b1b4559abd" },
                { "fi", "fbcb85c9410492ce8947a880a1d372cecbf95cb24e807b8b2a80a63d30d114115ba0a4d2f4ef9f0bf64b4c2389e1b9cfd53a1bfd33917479f0c50d81f491da6c" },
                { "fr", "7ac0d83612d6b4844e952432ef2ba8f796154410e5e2fcc376f5d464369b5f9b22dae9429e3be44d1d8f9aeac25050785f0ddd70f7cb0db38f958b1baa4190f9" },
                { "fy-NL", "cb0c14d13bbcbea83a5c90255c1f2b80e726f949f3b9769131ccf1c023cb06a5505a9f1185ad1d038c44de15aafdeb5a3d8fc80713595d6c8b1d8422c0ad0a2b" },
                { "ga-IE", "bbf05f6ab54d4dabe1e9563b2f545ccc4ccb38f566a1a13061cac723243a77ab566ad67d55236a18f3c7fe010844ba31845d002136414da3a81f4767da255ddf" },
                { "gd", "72ff77b99218a3a8b9a86add28fe85d793aebeee12e9253095055055b05935fdf7dcf451da25df9a56520bc1b50815c6a21f13990b1bee510cee982ae4782b80" },
                { "gl", "f79ab0da2c0e176520621fa6f246ebf0c3296d84488fd7fe07cbc3e975f2f6b58d2b7a9dc382c1a211c5994616fe0d268c592d33d7052889e1aea851029e61dc" },
                { "he", "37fee5f586fb2807f53274b180bf203c4f41a1d94087c543cff95688cf85eac04882f3238e82f3c3bdcc709665fa32f46d039a57ba70846bec36a80fa0972703" },
                { "hr", "78fa94e423b5fb6a355d01d6d9f60332885bc134a1a572f772a461dfca0ab56fae1fa5948b42a13e3c56ff234ca22bea48ee5f8bce070c8eaca4ad9cb1c30599" },
                { "hsb", "e15023ee6886b98c4a38819958092813b64d19c2d5faded44ca8ab55ca2614a351ebbdb687bd73d1381bcc2c64593c9026a3ba4f35c946c6103f8cfbf2538ad2" },
                { "hu", "743612819a1783003bcedc2f05ddd4987be063a12c26ac97a7b6cfb3d3381016ed7e63c60ee10335b5ba69f35af0830301a3db0ed16ae87ed15fbd44018aa678" },
                { "hy-AM", "746f344bb9eb6fc268c92c93a71106aaa19339d44a65cb775f1fce4c5cdf54281380368fcefd49c77afb9721ad316b9d1cbbb47afc6a989a83bdebcd7b312fca" },
                { "id", "3e5964ad24efe8ad3d0086b8b6742cc37f9c819825c39aebf59a8fb22554a81cd0fe188ca941da5926a04e2152dad56c17a38d6d9dd6e120a13f3430ab803f5f" },
                { "is", "55020e79fe47370eed5be8ed9ed25020a789db04affcedbc63b8a80fddb7cc78cf01fad043b0c7096f7e513b1440634d381d9ddf4dad7be772c29229a4db64a1" },
                { "it", "912cf5315584347ac7a69a391852610973d0c19974fab57dc297e40c6194520d74470307b95f4a376c907501961723d8a946c8120b13e646e444d0afc46cf499" },
                { "ja", "2112778f8a95fc87137044aa61cd489df23d923a18b3e3d15f0be6cef77eefa34ce622fbd03fecf6a22289fef43be9ac563582d870430f375bdeadfb705daacb" },
                { "ka", "dc6372e11464447bb77082bed5bbbfdd00445acd85eadcda009ec0b098f435e11decf5ea05c755220cdd1e3f97f41711599149cfc3d818bcb8dd2b16bc33022b" },
                { "kab", "2612e89ab853f76e6c139164b2e29ae7901a17df3f75fbcd2cbfc93f76afc67f0d21d86f3381fb90d568ff002f8feb6491209277321366277eecd19714cbde5b" },
                { "kk", "64688afc23b3cde776d70feec236bdcf5a5da8a6d5ba20f25921b80f815b41754c210bb1982cd24353cc929dd9479b6c1f8292ae0a0f1147a218a363c0f6b857" },
                { "ko", "9f4b5ae601c3e28e52be16b694d38d680e86b54f6c31c9a0696e8e9d057ae528817bdfc133db35bd7e2e60c72ec1069a47a1c71923481b1e6d542dfc6c51bda4" },
                { "lt", "62707fd1eac2bb3e9d40d9e1f97f346b37c01eb4c7cf543559d73c56670ddedcd3d9e76c546fc63382a5526e3b65f3ac7600da71212ae110a878127069d5e68b" },
                { "lv", "9ddc06ab8801d11c4e18c7166e0d86a4413fc7218e061f9e68666058a5d4e2492139a6aa1fedf6ea8dacc552a2dc04fe991b576a0daf5764035caa8411da9e38" },
                { "ms", "eb38038c8f05fc76961098d8a537250cc09764d6faba20bef63b69e031f2d799fb0cb86c92a112173136f6da07f385ee23fd0bb5748614e1df5a65c89a83a11a" },
                { "nb-NO", "9bc466a5f7bf3d77e2ec5bf3607f360b85b0a96e8a9006398e948d3db48b5e441f0eee966e7eed9d4a08bea686864d689621528308bcc8b0851a1c70725de55f" },
                { "nl", "dfd307c3626cef95f02421c6244501ca263a9e1a87be49528dafa8a0c517a3ba6c1ec34c80015e336968f1d089d83f6d5f00600c2bdd26bedfcd3627b29186fb" },
                { "nn-NO", "c15bc1dfe7afd2c5189bba866325e576a3ca7d36b0c12b53ca25fff5227b9e3a2d704f795a9bb1ef6c34ebc95135a342a77709e51542688dcfba75dcfc171600" },
                { "pa-IN", "e4712997ecf062faf83217bdb506a6c44f8a946eedb0685aa24cc1e9b2fabcb06245a1b2d632096bcc71e98c22f949bc0eddee861b7fbd6c2ca7204119e76626" },
                { "pl", "c66fba91e32cad0e7e6acc540c49f9627af54f1b9845a3c7c553c985995aea5e3c118d35e1b083e452af441a13b6bd89975aa451aa614dab2291cc0822b8d83a" },
                { "pt-BR", "665a4d5010e72497db4b0f9b0250892330056b573a35eb4c220de948263a6362a5a8e3421406f447bf18e7edd1402563c5c2ce4689e12084237afca7376d48c6" },
                { "pt-PT", "78c0777b1e4f660894e89e9c96f8c0cded9bb7e2f7254c79cb75fd20da271a07d54283bab1b122d0107ff79a2cbcc18fc051fee64517db06e81be412da5b1baf" },
                { "rm", "c6712ffb59e08242f395bf50455dc0dd9c6d6318cd0731236b2a25c3b3ae3932fd8ffd50d7762d6b038a4784702dfbb581f8fc713a6de09ee0981341be09836f" },
                { "ro", "18fa8b4d214309f8efd79f3448acbf7b076dacf97c9b71bae755deb336536e2db9072e8cac9a0de64d2120bb9a744c85f70438fd4cb3a41a6ddfc98e49e841cc" },
                { "ru", "b0540233e6d7bbcf62f9637f89eb2d551ff32ea508b538e39384bee02a0833b3ed9a6eb8ed04fb0f7186f5810ebb8f735c9f787b153c1529b2dc68ba44cf0af4" },
                { "sk", "a94ace6c1bb46c777e0d7ddc1002952be9733a73092e0a61aa0a91f80510efa3e6aab01312c79e8fe559918aae01f9a092d20bdecb90a6f8ff27ffaa8a0535d2" },
                { "sl", "ddbc81408a06349f2055cd5ff67933610765b3d0aae82c8bc96cf8c421f9280dc5b374782ead1708dfda0034c9764a05148fe25b136c5c116ce689436fc32e82" },
                { "sq", "0ea82a72b35fc27f4b2361cfb552cb41bb227f97df1d5e5c50f9fab58b533e17fc79991a73ab1006f0a438cf767c18a6e8de701c4b54b7d48b02bd0db3ae773d" },
                { "sr", "538650ce6e4aa9a462a54c8c819e6fdb4d3ebce9c8eb97430f374c59b4264312485917f30cb517151ac37bfdad8e0f33653f1d09d6695465e39fe2ebb3dc77c8" },
                { "sv-SE", "9d56123f69e717bc27a15211ff5d0b9c79e565ca9de773bbafdebbd174b28cc83eba8366a7e3e12e2d226064528afc5b02a6bc6647bfe3e616d1d055fe71a84b" },
                { "th", "11b4a0443dc8752796b0471cc0388c66fbd3044ee900faf4d6d5f017450d18610c0e6d1d3f1cdff1f4a640586ccb6c65ed5abbad1a5f2bd23f1ea7e04f3b3936" },
                { "tr", "2e0ba45668308e30cbdd8cb2fddb33635a8da553c0013b60d388938b64cf7b34f231c0434f7fbca573c903f9c2a1fd1f7ea55123752389a2ab0d73e71b12ed9c" },
                { "uk", "be020886e4c6cfcee0bbefabdbfe40e6a51a14605563e98f60744f782e49bd28da23f70251778eae1b5815ed2b93b7a6a8ea205a15eb824f5236575c47da08b3" },
                { "uz", "e985cd3c849f2f58dcd2bdc7fc4a76b300376a771474e496f2c26089cba06c3ddfe16236a76c360ab302e4bc7b196fce165d2cb333ef810ef5d75094a35a1e56" },
                { "vi", "e12f225eb9fa244c7b860203ef2868af7af7b56afb4ebbccfab927d22ec128c323f65bddf7693997c0e77fc2a89b7ecb0b5c2d6de21c3647dc702e4d7ad52a7c" },
                { "zh-CN", "93717304caa81140737e69b7021a571dcaa5cf93366c9b11a7b67b58340a3cf54e2e3a5379f3546581eaf0130b927d62a1afb20cf4a96374087c40dbddd3a52a" },
                { "zh-TW", "b7073bd069d663928b1eaf0eff045323be231cb29d095f1cd5e9ac0ac5992ef34673eaa24b6e37b748183bb2f4269856d2b57cccc464a6df5f775ee20384f5d2" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.10.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "27fcbd69be92a95a6388c66dd9b9436f6be4f96a6bfda133f14600255936ea369405a2a49ac589c88d72bbc5d1a27db626e200d6089f84a6d416fb4ff61071d9" },
                { "ar", "054e34d57bfb57700ba1cc671171b9876388ea5156b8bba394a59b3718738e48078a666d71127a351745eff9de290bf627725d00ba23a0663d1bc9b0222b7314" },
                { "ast", "470584f30752a5697d4b730ffb1146b6827a4b8039c28ca0e426948e5100c7c5e53604da7661073fc21c23a3cf8f18dea4d36b900ccef41c97ce5c6b2053c285" },
                { "be", "25ccf0539ff6ae1012668f148e7bda47738aaf6e525b61464f9562f102a3e7abc9e3aa6d9e48813a0a5a36483c8ea0853d76c56d26ed12e4c2ac45d322880de9" },
                { "bg", "c1180bbf5489511c9cfd499f2506b1d670e49896b471c1aa1eff71ceae5f5d72663f0816d1338077b737ebc2656e6b53689f0b31abb513f23612d7e78c37d0ae" },
                { "br", "063cd16ce582ae657d69ea5d494e71bdfb338a2d0013a49a32a81f1e66c12387c7abc88930a3e9b39cb9f9d46eacc3549a4d407122688f551545b324d7fe15fa" },
                { "ca", "d769e700d490a463b4adbe15ea8bc6c271aee8f2a126fb473b6787ae63223183254dac15977fe4af5ffdebbb140b55255ce29b5a2def3dda2fc2479a47351d50" },
                { "cak", "e6ce14f4eb8ac7f4f377b80151279e0d46f7425e737b8b9edbd39b3717fda9c6f4b0eae18a8b8215d0072fa89fc777ce77769247090cf3053b0b1cdd16c340a0" },
                { "cs", "43b3fcab113fc6c0c4951c162e13d3db2f12ca131e550c370039118ace82e1fff3ad5ed84bb02db4dc380d6a4d8935770427d83a3af643a2ab5dad1d2fa99ea1" },
                { "cy", "c9ac0f07991e7440374ff63d1fac149b12b0ab7aedc54f107730430b249c93d0736ff511825e9b535fb6ded5c97e8ea345da38e3b0fc4fbf500b4ee21c78edb4" },
                { "da", "aac364c5e67a96b3d1bd336083739efb0093cfcab343c50f47be65ff1a0c59a9c932e76cb8c09f8678dbdba5bc82a438b3215c53fb753fc8ebf558d9f48ccc0f" },
                { "de", "a3eda0a8d38aa4d7a7121ceb1e71ce9c68c254fe015ea06c1f4de955d115b9cf9dc6650396fce06e3142f5ef6121294cb0801f3f9b89206e80c9159a32c9fc7a" },
                { "dsb", "0fb8a7f2f8900c7d2f04d0645f1a09d1e0973ca98def15f5e7d1a8c652f2aa9f6aba9eeb751f8c56af5affcf78c9ad1bf1711bb40e637235ee7881c180484d69" },
                { "el", "668c9e6a31c7fe53800d3a15712e9a4a04fce660b492d5656d67a05bb2521d6bd61a5112a1542bcf8e84f8e8527950d64069e585463f337715e4b3181a8bd3ff" },
                { "en-CA", "c87e320767de53bce38d9147ef405061b16e9fcf5b455d54f4cc21addf2fb579f4dbd8ab9d6b991ba7d0cc181f8fcda9b74b8e9d61d0b2c56b351b8abca6c416" },
                { "en-GB", "f8209182bbfed9ecf0f9d507a07dc6376413077159e5449b69692d1fafbb22391a6c41a83439c56cbe39792f3d3794bcd6783d4eab268c560e8d94b07e39d769" },
                { "en-US", "b2778e5d913f62f795541051fa70c2450e3e6951b00382f95a8ea74a04a3862fa73522e278118a281cc87cfa3b182e8b613b71060e4bd45540ebf828d6c5e80a" },
                { "es-AR", "d06f1f93004cc54e1751e38d3f4bff3574eb3a8140ee872db79021fceb0caf0de88363c5a0c1fc6af7696ce7a67d88336de0aaf5e0e73f2de728e27168dc7322" },
                { "es-ES", "c67dc15f6b91320eb75c4e60300820fdab33215a486413ddcf521a36b0cbda0557af77013e4e3b02053f36e2cc0b9bea069ee0c6d78e88b1578a7c1e2f4b13de" },
                { "es-MX", "ef0c77b1a55036a0ab09543f96dacca6e5c40b8df2790af712e7b5c05022bb374c78efc1f68a3d6b19b4775da43ebb81399158d0236e45c6ea21fc77891ab6ae" },
                { "et", "6a38d5a1bbd561a5dff429e4a8a7099922eec2a89e05839da549d41539678ca91cc07d8e4f8561a01809c535746663ca022190693be3405923fa83c34c2ae13c" },
                { "eu", "ef4fd5b90c573d795baeeab3009b0e87dc8ed0190a3efcf83b0b4886de40dd15be74ccbcc2b600e1efab6dfe53b4cb198a06e0d4376e9065b1fd967abf764d4b" },
                { "fi", "202851afa3eb8de7be93bbc12708282896af0741d667f00ac59e9882ce07df13c370cea5a2cc266ac4bbcc7ef6511eefd495bf56db23c8d81715118ddc7abd41" },
                { "fr", "cf41c6ab4d5334f9d466e412345551876b9963218fc22460f4dd2d885e68c28769518463c62a56fdd9c42abc96805e09e01714c5424f63982509458454aa600f" },
                { "fy-NL", "b17345aef86b672437a6cb9fdf8f44455425136d0e601c9e47e5e69ef6429d2e56e9826dbf151ddc0219143bdeed2ad1d205ae2e67d0c22ee5ebf08f535f9654" },
                { "ga-IE", "aad802874c66dfbaedf6217411d272c41f72f262adb10fb19dc9b1d3c17cc9656383ad2637c23d511911342d86c9640a9b74f8df078a3b0bdba421a6a2f825a5" },
                { "gd", "301ac61802e195cb6e70c1da4cf67989f0cc1b3bc0123daf66a180f17425acd2311373f9eb019ae4403407194f5fef2fa121b603782af6f2f8ae746e5691b5ea" },
                { "gl", "2043925d4762a3d77ef598dfc513a0a100fa6544a0c533813f12d97c98fbc1c79382586ece782cc7991e75a30713f0ebe4c8efae2c2a308b01660240a67c2c4f" },
                { "he", "7d0dceae42d6df466604b6df3af1c03d1d81318b9e7863f6ccb58ea1497c1d1b4085ac6f5402681b784d5d9db1976ad8d4f73829230457538be7f9e3b0a8e4fc" },
                { "hr", "a59126e374c27e575675326b4aec6a1a47ddd0586e801cad66d9819c6280ce4000ce80f65f35d686f21a831fb17342a5a61b3979e804f0a159e4d8c511239a0f" },
                { "hsb", "4ff6f6eaa58f28b79783cabe43e296fbd9b52f74da1f9fc83981df7a53e93e4320520f8c5c03515046d272f515e05526c5e8c2dffc605b6a42ca8a9fb4c04379" },
                { "hu", "1fbc6297d54bb34a7c3162064e28d164632065209227cad064c600b5f25706e6fc9f9e76451239288765729de3e6883dcd2899067793cdae6ec64f5621aff3a7" },
                { "hy-AM", "928fa20cad6f3d574bbb7f13dba735893e1e37d8233f3b521c0b46baaf28f6667a4787b98a91d36c69c1de80d62795bdf62b03896046f90ffa36768008adb769" },
                { "id", "6abb62e5e760ae653abe49899b9f26d1b6d6fc42ea8abbb8f45e4f4e5f543c4f258e87c721ead493c6bad6ee2e4bb323b2bc9690814254449c7308cc449108c1" },
                { "is", "52f61d497189cc90a984ca28b674455547f707d53fa6066c57a970eaadb8fbcf0624da74217c860de9d79a0afbe0c58c5c0567b2fa55e675246d153224d69cd1" },
                { "it", "473299c7c40a7a94a40aabf5814e524f0cb2e0ed6465bb74df3d4a40c9f47fcdfd589c6a1d92db5964983e45d48abf43c148cee49a3e8a04d58d1526e6b8329d" },
                { "ja", "b6046ff74b08e7e2ac88cf338a93a4734085dec839fd0a8207a366005c4f4ee98f2cbfb41cbd353a3bc0966613c808ed84fe0d428b2d35e46262dfa71745e04a" },
                { "ka", "b71e14d0e77626e115f5f43f23e1755990e5a9fb15976f3e138058f5c66c6fc6a28a8d977be58ba2881a8fb4971b4070cacb6524a4b1d581fde60a1048caf414" },
                { "kab", "e529f98dca5abc41eed7b032e4b17c60a6de18bd0eaef90837a5687e62ae7c061b53df969ca5c945263b565f569cee31dee1c6784febe75a48b6a67ca045532d" },
                { "kk", "92399c4d9a617c6d3edc4d80b3322abe0b788f74d4e6a7e62b5dcaa1333f339624b3fbce92301b4ed1074511fa96211efc8316bf71363bcff8d517746e609d29" },
                { "ko", "57d286cdfef601688fa047a98019e5731a41d4408ea1a0956f2a0924d1570972cf02d566f50ae7312738f135a80ce1dab6708906d6dc3bdec10039330efeda8a" },
                { "lt", "33c17d40e672c1b0847f47dc84d8e781c696bec7983d367bf007fc6ebbc33d56f4fd1a970125684adc263de0875df29d97b0c9d70c257daabb0363bce27b6395" },
                { "lv", "54a006214a3d8fc4000d3460a539d26a9577cd3aab01a4817a9ca8a8b051d5ae8ebb5f03ffd0f81a43388befcd197b3bcf58cf4e884404aa3927a0501866477f" },
                { "ms", "462f9dade45473c11b08ba94c516d25fb510810c00759d09ebca59fc20d98ae31709db6ccdea62da2bd3a9717267141f110717c1a77b601729ce91eb882f2f4d" },
                { "nb-NO", "b9e404d9d05e95a52e5b1eeeb4672f904325c758daac8bfb20e75507129b24230b4d8616e671703a1885638b85a36e2a5bddcc7ae0711f8a5df896323c33b452" },
                { "nl", "a6d5c14a7b1fb64b67cfb63b28b37e86ce111a34bf0c0d68412623dbf4a28d16dab53a892f67b0f5547db131fe90942713f125b77a7de5ee38bb0244293774a1" },
                { "nn-NO", "28e0c7d3b1e2e3f779d66203a63cf24fb516f4425760d9ea0767fde788bf9231c6fd584c6387b2d7a0b5ec2149afe526fe106272d8bef3c3f277340bd0a54faa" },
                { "pa-IN", "0fed0461ce2842539bfe87ba7655c526c2bfaaec735a97571f43d49448153d338a8dfcf314fcf26482d2d3c46e626a34366823881f950332153552261d938db2" },
                { "pl", "e1b487d736332569c65b5983b5304395161362cce1f73e9a25134ebd8fd9827752c5c39673d03ca5f4621b75ee27879aec71d5a369c04d9ff80148fe795dd80e" },
                { "pt-BR", "0d870fb0d5c5f1052281a48174ccc35131a75c4bebec3b767e7952bfcb6a7019cb2b3a3d1d6d1aa33ee44a1478deab7c4aa1b804c9ed7a1c5a2b9188d1362ff0" },
                { "pt-PT", "1aea0a72e33101e638889b1a1c97af56da6e8fc107eab8ffca0147dad82f0aa276ae317c81f5b5a3f914f4ca4351e0abc52a52cf6b1b49ad5fc87eb1e3912e95" },
                { "rm", "7f2fec9df8b492986d61b0ca37d4c9bdce1c04b2cbf504a4989626d75c17e5f3ab73a24b9fc90b642a33a399406960753e35fc3f844ceb74cefff4b4ae6897bc" },
                { "ro", "98d48365c4cfb131c61145d628f895503549ab16e158f89ec84b21fd83d176df47899b1374f257b1f0af6429152632cced71af27ed558f225459f9f7a29075e8" },
                { "ru", "dc1a4c4105516a0adda72514f166ff7e495d8cb9b6146ce131362bee42ba3fb906c10087505fa97b615a6e1d069b0642dc80807bea25a906357aed21e2c43ec8" },
                { "sk", "046b0f5d8f583fd2f8763987d9715fbae87cbe5151f1eefe740a84fb9f33df97720c490339374259789ddcb089107658ec75f9c454847efec549a4dcb040432d" },
                { "sl", "e6743427f89b8f335e6847ae1d47501803b34b58e127541403bcc39775b7b2bc0f4cfcf7787808cccd76745e9899a9204aa2e213e92104ce6ca22575f5ef6199" },
                { "sq", "8979fa788afc175d66941d10f682ab63e856f765572d863d217402d08a310ec17374bd28800762fe1fa27024f21e01634ac6149014484ee72af0fb6459c7c3a5" },
                { "sr", "e4dfe569c897c218a39c0f035a5d966ff9b509ef880c42db88cb35243abac6748783e5a958ccfd75e990a07e8d120ff2260b78e5a9308734cd1c07e16ea235d1" },
                { "sv-SE", "97f37717dd2ce8b6826a1abc5b8367bbc48d8a63f0962c11e9e36d6f82d940d30c810d269600dfdec44694932afc46becdadc68ab83fd7a7b17b1d9d050de15a" },
                { "th", "8a37734e41fb2703614991eabf012f4f2383dfde7496b3a5a1a2b9b7f31ce38bfa33324a0a2687d91a1d3c8a216e52a95bfd207e70e04eee8542700da431ac34" },
                { "tr", "8d9c03f332b609bac73d03841be3956e62ad6883b93844836e3d64bafaccef26b7f5fdc56e7e9879680ecf0b01f9d8c006b42a1c5bfff1db7dd9cfa3b2730370" },
                { "uk", "a86a5cee4816cbeab41ccb363b02e1b2af09aa6ab0ddbad8c1e3000db2149925d9e7950e9041b1f94766875a79c68eb39f9e43b0bea7fec3b2fab01d163efa3c" },
                { "uz", "84dcf97faaa3a5b96a6b2661f3dd6b7cff289c46eacedc9150e454855011d30390824e44ea521d0cdb8c27dbaf1a0d6f9f2e50d7bc0565b22bc2eaf4fe20f732" },
                { "vi", "9178e35dc4bfbb13f489f7d32cf517f7d1fd75f32664c5d0a6428055cf68369163d71e10b214b0314d7339f9fd96860e6e773d6031f91519acb405cd74d6add2" },
                { "zh-CN", "ec34b8828535d96b2b4967b09181e477d8ccf47671f02d7e89a5199dac49c4c8d84cf0975c877093988923353fc339515b0bc4f4c97c25119cdcaf3a749b27a2" },
                { "zh-TW", "0447a495213405af3134d3c00626118710699a8866f3442088f73721b56d2531bf84c25797ec507a2d60934d8ed14dbaca6cd43eea9f693c961b99a3eb5a9ebf" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "115.10.2";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
