﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.5.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "f9e86fcb0e4f8a330e79636162624efb466ad0dec8ea1231730f32e83a2b0eb9f2b4df4d60b04ba50eeb16b77a876bbaa54b84820bb9ccc2842d2ed2650c7f74" },
                { "ar", "8f527520ccbd824735ff6d975f984da86b9e4498166d2d61a6673f3829b62a4c47248e25488490bbaaafca75b971d78f5eb55a771a52aa6a5a1faf20797249c7" },
                { "ast", "cc1cc6b7d3ceeb757d9b9837dedb12c274f1ddcafccdb1af20b387f926c1c823c55db52922ca78c88656d878f05e67a45060f118b059aef04d40738290ec6999" },
                { "be", "caf0e3270f222b7aec8ed734e81303fd30fe9f5766260f0ddfae5349aa3f464430d73728accf74e3539b1cf85d73c6aa74c09c750038c0c9ac9ec1b93fff4482" },
                { "bg", "aa1b41e99bf10fd50813a204016ac5a708a520b888a9d3c56abdcfb58b7b3584ff7fe9c041652690cc4819c32296abcdc14c66a8526e91309033bca38ed240e8" },
                { "br", "781f7dc7ecd957a2168575c789f4481d02b983d76effdd9467dcfb631e8c9752e25c23b13174735eb1c930ced8dff4792f80f25ac1f70bc04c65ebea7e12e9d3" },
                { "ca", "9f95e9df8ec19cd39fd2f09eea13137a3056a765c2f0b4faaa0a07328d620fae20fa4ce9302ed254e97a7c7cf37b8cd75970ad299bf2192c621623d7d40d1f10" },
                { "cak", "0aa26045622819437c842d821081e9bcc5a61e48cf61cd0533c831c5872adee3658e2966d6da3f6d98e540bca12a15d11e55c9ce198fef9cf5c0c6056befbc31" },
                { "cs", "3da01d067ed6a0e77eaa30c2381022015a4a76688c341c3b7279d29550d6201cbf7726eb59018383ce5a066c82fc3122249db22c7c07ee8d89be868ab0fe63b0" },
                { "cy", "95e9061af4f878cbc9dbb2560fd73844cd59bb37e7fdbb1d4f0f53bbc73bb1361ed5bbb101510996f5e6a248cc67d6b97c40877f6a7375ab9b3f79029a70cf70" },
                { "da", "c8e70a515d6dadc060a78d8bfb5d54a33c48f72422e5561e3f9b077a06e5f787948a9f18c214aa5e5d3d3cde6ce3bedf9f3bbc8ca320e491781046d0f3f984f4" },
                { "de", "5695eba6277c1795ffb2ee7ebb4bbdd587d1c5eedf1046ef094c77ef040a8d8ef7d8ebe829a6cd1327c7977740616d45ea1d5d0ce792276f264f88a28f3f9752" },
                { "dsb", "e235e4b7401feb3fd3d63cee73836ef10247078c2f101878286c10edf2760e3291beff5361837cd753a4a461c2e7a7521d9c8eab3b1c1f293c1a91173b0f32fc" },
                { "el", "43faad33b32b7d2359516cca62c51aaaf8495297b875cc120df484986b463d69edb458a36789abd07c2f76387230845bf041e796bdfb094c4367fb0869f784b9" },
                { "en-CA", "335d0e1a556f9ab7b334a1072a6c3cac77b4989b88e4fdc6085189a1ea1d2e43577bf2d19a811e493a644019acf49974dd7ae60adf95d3ee3f6e502e5eb65f26" },
                { "en-GB", "2c0c781bb96d47aa3d0a0805781fc99a477f5bbe9f87be50f03768a9c3c883268db831d22e305333ec087b71f1d8ecd1a48e587c18b0eb1018e55d9e2f9c495c" },
                { "en-US", "6471475e5e315ec344d34ed85fb2bff21c726f73e9f39cd3f39f73f50b70007cb556da8e6d02277e025720403b5f3a363e793fec657f28f1e2b17bb2ea4007ac" },
                { "es-AR", "d581c38e3714c6b27fc0c36d0e4e1519643892076e8b74a6b3049b49d7f0036bd54bd133ec171a0e44a68da3983678defbf3f2988c8ac58061ae682383df5024" },
                { "es-ES", "f30e8e9b4382cf145a4eb3499d8fd5abf7a13e8875d3f369f8a3a14a9174f580055056e1d6cd1f7b10ef8eb42722547ebf135871cc4a2dbc08a7fd8895ff868d" },
                { "es-MX", "8fec09d336f0f7063e42317a3d170d067b0c8bcea8b3d4b1c43fde77c0ba2d137c318af5594ff4b379a83cebf0d2cc803eb88b18114b3afb549cb384db075a95" },
                { "et", "9d4f5b6fa96e660f6fbef8e558b7e665e754109f2ecf48dd29e5ff3b34423062a6f43e56c5a1afd3e59afda385bf5716f9063c0b70e0bbb0757f3c15b6b32a17" },
                { "eu", "111dc6b3ca2e50efc51f9f3b3ec336f29eb84efcd0eef75b39ba57937c86a458810c583dd70a5ff0c879d38eb7ffe55fcbab4e137461de759e57190ed0916bd4" },
                { "fi", "535bd125c58fc0866d707080a01196ebc97851ef8a5468a530a2007da9669d333f9a1d57d11e64e79d3f2c41f486868fbce6b919f1e1354d6480c93a369358ab" },
                { "fr", "3b9aeff6507c8c9babea39289c6b9a81026064fbd0445e574d0a0081a7b8378927346149d54ff0eb3c275f3897c9d1604c6c39928e0d9cd8e4c2586c882784e6" },
                { "fy-NL", "afb25ea4525213760e2c55c87b2f371fdc52d4a7e028e5165cebc5fae136ffce94cb85d6fa5bc20efe7069941e5fce474f912c1e3a3998c31184fa02e62862c3" },
                { "ga-IE", "e3c6bc720ca445303f3799ae21e725ceb354d2407a5dcab5b540f133e82f615d1d2858a57af17fe902c5fa77f972ce4bb73db3db7fa8b708da04d5e25d97c47e" },
                { "gd", "5b192345772dcd1acdbbbadad244ecf3fe16ef299ef59858297fd732a28aebc0a8b67420be93652a75a287bc84df8989fac26ea85254548c20e775ea9bfd4156" },
                { "gl", "0a94928f1901000ecbae0abc5885763a12b528b3a03bb8aaeb55d697715ab7103fb4916bf1d9f52a987bfd5140b275f592c29d451a9a886f21bdadd551df0907" },
                { "he", "c5a5ff213c2912b9ce913d8250bc09ef274babaa9c68f8bc54ffe9a6deb46a99bc4710574577498bac86ba4cb11ca7189a6d85964270af3283e46cb1fc38ae11" },
                { "hr", "6274e3dbf866582af2693d9aadd37fd3923f0a02d80e7f488029225cf4230df65e5785fe3721b67c357302deb2717a5a46fe541742a6ce955d1a993e820d830e" },
                { "hsb", "e98e4d7a426f57d9c12c0b0a120fa369a0a1dc1444e8f9c2bf63064afb519daaa6ace00abbada41ae1ae33261169986e39beaf2f3bbf0dfcf3fd67ec49a49fd6" },
                { "hu", "0ab94810ac37436ff145a95d6735c45a5c4c1ae8c84c504c82e9257702537c7438fc8959587b98e952196f0d4008d2aad3f35e14db6ae1ad827c49da42dce99a" },
                { "hy-AM", "8e2baa82a34ad4590a7f43e22cb0ef45e459e6ac6079aa1464eb513bb4d2366be2561ce29883f3c508f854c3e144ebd6b6306211e077b9e47ceaf9a86dde3c08" },
                { "id", "6bdcbb0e2d4ace45c3d71fe58a6bcfcf8cf2c581e0334da0f458c3dac906fe7160b9e772418a4010a3c715be2f575ac3abccd598826cd4e1a2bb31d523e1041f" },
                { "is", "67f1043d8ec9e79a82e16fb0d4ee45a780c942d5fe4ae7046addea4d7d961d3d12d192c7d43b0bdfd1462a265052fbd5a7e7b24b1f29488a70f8b7dd1b891082" },
                { "it", "8c8ee0e70668bfd05400d312357a702a6c0be2ff64903b3fc0cc2bfee0b147e42ec6ff76a78f11e145fdffd8fbb2d20a0253ea5c70d0c763f096370ab0dd2bb6" },
                { "ja", "eb6f6650aa48872643847a7fd5ed61c7c0e9c0f68d2e8f4ddef4fd136b37b84be5b25b042c4eb78a967c92b3906a62801d47cd8115efc46cced8a1584c36e87b" },
                { "ka", "2025a0232ae5a3d941cb01dcf6bab8e46c704c81bbe8f8cacec3510090476ba46ba64560fc3c571c21cf5e6d1b09465f332407eeb38047140a18913d3457237e" },
                { "kab", "3b1f2a206b828e96fd51715f395d7fd5529d354d5548af492a1cf1be488b0ba2f45a576a377ab031a8ca623b3a8122c1611fcf72ba61956a79107682ba751483" },
                { "kk", "aa560c18491879d7d1851fc28fff9239c54433f99520e66617b0412f15438ad24b3a7c367900e98831355b819edf22db8412fbd0782aa172d796b63c678ec311" },
                { "ko", "81b96e74b47ace81fce6d5a69a23cde641c971dc09a18bf3cf85d63c6197560d1087040a715ddd281cbeb483379a22e2e91c21e3d233a0e0db0c98a430777da4" },
                { "lt", "494904528acb70314cce6cf2ad0a2ed06163dc698979dd9a186283bba1e2b88878f82398b7425107ed6054262d6d4c9b546ee3081db223e7047fd5a793f56e13" },
                { "lv", "e3a18d702c1221db1b074c72a861a1adead0219fe662e541fde66fd57c5d94d15665934b61491a7bd105729567758af85aa63096e98240e5ccab22407c2c1eeb" },
                { "ms", "7bd34a9c0ae7f8df57c1ddc75b1796349619554657446d5f9aea598f8fbfffa2192dfbab4ec12ba81c26619d2324318db6662cf484044d1bf6a655a6f839ff13" },
                { "nb-NO", "16a82b7968da5dbaeabc218a5693e27dd83a2e4cbafe2cd601e383992e915f584120e769529235b7a92cc253c7138bc9f3481d729cd4bb435efcd9455510eb2d" },
                { "nl", "b89df59c679d0f7fdcbb6db4dd08a7b106736ce34b7295ea11bd302bcb4a996e43b44dc69db4a7af26409bfca48e75e2cf56e869f9131d5f15eb0c823c6262bc" },
                { "nn-NO", "ecbc7370a24eb232e248922f3531577d66a1089398a5cb94754638fb965025498e22e5c64a7b8e1d2d16ee2c1cd937bb552913f4dcfa914411abbe0c01197f36" },
                { "pa-IN", "3bf922ccd213689c18dd55573486d955a0695d06254124cf265f8b484398d84a1fe961be98601850bbd192b6f80873be29ba343db6acd75e16aec0896971e0f2" },
                { "pl", "0e456eb4c28fd23b0e5fda02f007a20b1dd9a53fae80ab1d2f72814c7f9094bbd46a3fb9c25f5d426cafac573167bbc550a7f1adfab7ba3edb14feeb29009132" },
                { "pt-BR", "b52ca16dc45c0318a37d5aa68059f81366f47e3adf61aaedb9faf9ae27cbc0567c1a567601504f73ed7b11d969cdd6863c9c2fad8b9e0e27aef552a3031b23cd" },
                { "pt-PT", "1d112b7b5816760ae9869e3aac47d20ac2cf22a4aaa9044153682b167ab087e9696fc022af723954638dea57e1e497ae4ad2f228ff405fdd12f955638456c83f" },
                { "rm", "6b659ca732bd140a7833a2a7ecfcf61a89960c0cfb0e8e4d491d141190ea2423e6fbb279142164936f1c191193f87559781599621c4d233377d44bdecdaefe0c" },
                { "ro", "bd198dbcade8c5b4e1de87af2dbbcd518d6126ce457819a1f37ff5c32a93690c5fac8dd17dd12ef04fc8e3f9ab1fbeddf8fb9f625829eb1eb8ab95b0614f38ef" },
                { "ru", "3ee14e1808219dfcc7237d1c04f8adeb0d243e81b9796572dfd658da49df6724c26bce22b9484f22f7a38486053f7b18684907869371e28e7ec211fbbf9ec570" },
                { "sk", "77a63ea97c7b117f58a7c9132dd281e13b2a6ef2fe070a4fe3caf47ddadd6d89f0e5e74c0e42a976068b0b0398d2e7ba814fb774547a6238866baab796b82f4f" },
                { "sl", "4b5ad6077f45770a591ac9d81b2b0e6d6f678f5b8f2afe72646fb65643759c2281d7d6123029b149597947208f50035a11ff355c8f754d9b2e3e34054683828c" },
                { "sq", "76ab40f6610f52ea129a02c2c49cb0f05cd7c3344fd3624bd565a32c38a407ebf322d9ee8d1da344e1bb323809904d1ead19572bcff716cb88be64844136f2fd" },
                { "sr", "4d191396c4dc82607be5065ed8ec0cded00a9b870a4df7a06d813d5adc0c26590cdc17d7f9909d9b76030511b72fc8a73958fb038f126a60c7919305b2a725d7" },
                { "sv-SE", "c3f0af657111034dada17fa6ca18bebed677946a0a503120b69bc75089687871825ff2673e98d367c7550b368e9fa1d9d042cc715115110715eb8ac91ba1dcd0" },
                { "th", "3f72a587ce692f05a0a4a011bbb4e37f7867ca8e9efb120e63e35c9bc231c5cbbfd35a93374673ea77255d9e03a7161899b05e43b530b093ed1f38b7407d8d13" },
                { "tr", "91d0d66953ab64555b3b11ab8d20c78f4a61ea7217494874ca49b74a0d97607fff00743fd8c9a241d14f621db2c7f61d7ac3888b726060c8d8a9cdc284c9f87f" },
                { "uk", "da72f59f3a1dec1f6b65b665e2650b8dbd52e618a0f9f0356170bfb0a425b7c5d8cb08489582bf09f533400f63c4b21e8bef185263158833ee3ae6c543271120" },
                { "uz", "df4afb79ec07deee39ee15b177308bd0169bed0e19bb50b16a068906c12ed118e1826a181fba496b45a2d9243c00c97cf45a5fe23b2a00b76cb328bb25be194b" },
                { "vi", "a4eaf5ceb32eeb536647d1337fd5096c6e78f0ccf81ffbe537d8a391a8e33773d9695cc10a90cb1ac1b272d3436ba3bd8a209c896cf2065e4cca449c6b3d9b41" },
                { "zh-CN", "5c54e7e35c9829246e0233d4ea792f3c5cbdf8b9785b39c4a6d882866695a9aec6de8ad0fe641424e7c333e67b6dd2e376b67121fad3256803d495ea39ab2b66" },
                { "zh-TW", "13ee9afab761b7ea67f1bedfda3d89917d566f9a0483182eb36731b2899aa984353eabd854e906a560d9acbe6ec376055d6453537d4cf926ec5658b13a5eaf4b" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.5.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "daa9d0ec4f15b4b0dadc68bc1104b68df4d58a93ba5a109b4457451eb84e2138ce8e15a3db897cf21e36be58fc65962b34931668143cfd8b8d32c5c0de53b301" },
                { "ar", "647815ae70b89791f683fd39e410b9c8c26a62e373526824b3496eaec3be01ff55aedcdb6182730281df2d3676308c302785276086826ca034ff790ecc3d194f" },
                { "ast", "7d7cd41e62edfdd48e00665e0fd8379721c6d6843ccf6761265b64600bad2d9f80ec0842fa8bc1f4685d696910975162f85f8569f73e04f4463e3a13496fab9c" },
                { "be", "a272c1433362e5225972390cadb576baebf85dab2cfc6f96df469665416fe57c4bab4753ec988fb52d524b9f8d13d422ea1d4da915eb9377a11d3e945c470a3e" },
                { "bg", "3c8dff0997dc2f31b4d54680f9045ca71961a3c0f347d2c8346dc35b7f511a941db429a6bb5d124916669ef59749c62c251534a6ebf734232a027c82f9cad2d2" },
                { "br", "a1e8d2197a0b45995189f9c52041add47e52082c00d6bfc673bdb41461d408e7182111056e290374638007b7a77a49e6a515058e44986e4b4afb484ccb3c7cf0" },
                { "ca", "070382c40f6bd1f912273397587fca5cfb8fb5d846a90b47cfe935b0eb8d3390039805d24b458e586287fe4c9ba22743f4aadbd428156a2cc863720c05ea80bd" },
                { "cak", "86d75d7b4c4459d3b099aa5e90620f29be0c8865e2f663826727d53d38abda73252390dd2e5650dd7e10f831db4c643dacb29332082df483554ba56c229dad5c" },
                { "cs", "43a1b7c90018c0ffbeec4d13e92f45cc9041c4ecd3e5e5743942a7dafe51fa8cbd9457e81e139a9a3ea506ca6f46acf44f7704fbbbc040a6673ee271273c3cc6" },
                { "cy", "d0094985999d6aece4c711c64cca7d9cdbc14811e2f5ae4f732ae5987a486f1d1b8fdd32018a57f6ef0f7d84b83ec25b677a26b5e0c3669b9f4bbf9b9c19ec66" },
                { "da", "3e9f556fc657ee641cfacc11eb145330ba4b58ded4b45590621bce8163cc74ca91bc2b9bbb80541d2477abfd703373458d7438359fbde2d9a465b1c0c518c7f9" },
                { "de", "8dee02196537e6734e2d14b00473fe8df93b7547a91c7f5a7c46c8ccd7cc9c864b3e3e6554e311c90f3afd64bce6d041c3f5df5b284036aa5a6b5821711c57b2" },
                { "dsb", "6ff45063c84aa71680d0955fcdf5d892d23d06974994ef9384a8319aec039b695f0dc842830b40eff773a626a62fab0e77d3beea836e549b365757582f3b6cea" },
                { "el", "4d5c6e53d6949d8ac1bc065f2d7443b5f419c46929fe915425df67ca39354fc4b8deea97a2ea2bbb70b1c9a5d185b0f55d6fe660bfecef06b4d3adc13b598b00" },
                { "en-CA", "c3630da0ac5b86f0f86df3c30e039123a7211cba5ff4707a981efb518d6a7b042a5627a885e61ef27d900c8d23a00e5d02a91541035ec4a662aeefc13c685caa" },
                { "en-GB", "ebdd8e6e3d01c634237f9e2b2f241745a19969d2db481e5a4b345b41d1622a510c8be972446f540afcf2cfaf0b676dd5fd6c5d443cb545eed2ec85d840daf708" },
                { "en-US", "39af632f2fc08ecdeaff5ccdbc296ad79d3b8aea3de18a54dedd73c44a749c04516dc97980a9195ec7e9ed7f6677282a47835acc102ca7480d190e1f4c4b6f72" },
                { "es-AR", "63069b39baa1a327361b54c9b12ac5ce5f7080350df725aa89b1c0825e2e621b0112906245be3d2d24ce35bc0c834972901d8c3b88f63aa318e21761842d5ba3" },
                { "es-ES", "ad6585045ac34021edad05a79ab3c2474f7ba4ca60a934d863967a63a1218e4dd7f91be5efed3057973387b066c1792360788ef36767dcbdac00e4ea44eefd36" },
                { "es-MX", "e940a4b1cb89aa774659c3a8afc67faf44794edd7058c7815587620640d15df59feff6adfd1839d7439eea497340f7ed1a1b11f9ca3e1dd62e5120f58ead039d" },
                { "et", "d16b49a002b18698ed5e130001edf21c53ed14d43440660c9bf4c4f22a8f917b120984849bcd78b39f8c78b4d70e1b9eb601d2b180ee428e89ef8867cc6e325b" },
                { "eu", "1f1a96acd9e0638ef77d036da633fa50bb84a547efe11dd045bbdc8c9461a26f829dbde8f202a885207d1a5df17acd20c4df31189e0449e6348fc97e61f05ff2" },
                { "fi", "4c117353b428f73e7e046cdf318071d3c5f2c50adde2e0c06b10a726fbf7e87f00758d2b69b1b59d8db7a99e2bc29c90bb191a698dc3bb4d7f397dbce1e5fbee" },
                { "fr", "eb22eae5a52197739a0603cb591d8aaec17da398698fecf6507819a1bce2babc96dbf0777e41145f0cf58ba197fef8c6cf58d898699f10f59c9e87cae67634a6" },
                { "fy-NL", "efdff1f117cfec1024567db0c9a0fe56404fa40a1768ceda660dfad05d5b0aaed50ab1bc4b4e302ef9294d0b28d079acb1da8917c3fd42df5ba12ac8e427e69b" },
                { "ga-IE", "5e1ae144cce7a64931d173a084bb2607d5b046ab5845ca7dbf12b10bebdeb2f82eea89a67d0992a1e91be26fbbf7b4bb242a1b9df6063db91cb879bea474ec7e" },
                { "gd", "44b6b01d675c59941b4a34b25078eaf63ecf96be65d95d49fac6b227362e75e5e7f9e0fcdfdf9d0372ac087f2d0a2a1a8cd774645d9594d891c91dbc512b5df2" },
                { "gl", "f033194288ca2e50067bbf5f83d7cd2c3c6b2b99a8505b7c50591626c41ebb39bc615212267054fa75ea4b61623b108905ac0d3d58bc5067026b93743b36ef48" },
                { "he", "48d0f8241422023b62d9b8eb0323c028ee8866ef10e62aa6bdc96e839dd7597b3490a704bbf7e1426f9a30e69290d5aa3c07862391e55613ddbebdde9fe55b5b" },
                { "hr", "40752999c8e81b00a9e3b7cfce6d0fe7955af0027fb3ec4d67b11f6984b0c62213e42b6e4a2a710f94b2a9f9db40bae56adffefbe42b15055e45415d8a14b3e4" },
                { "hsb", "a7b963233c135472b0fb814b9841bf2384421b782428f2d5c6269efc1a85bb17e63609bd3aa8b1934182f6d974b31d98c8df23fd8febe23c2cc9fc3fa3bae7a7" },
                { "hu", "741899424ecfbc30c9c7af1d37e09ee1546410a5589e0965e5bf3d89344d255abf67fe415074ba50858278697bab0ad945fd7b55b580b4e143ebb68117bb041c" },
                { "hy-AM", "6a810653dd15d8561436fa6e71211c29a9481972e8e7ff25c8fddaddb36e3f409c7b469cc37913a351ffb27cae096c203975fc2d6b8708910f6efe08b98240e1" },
                { "id", "cd878391c0eff7cdaa8623350788dae3ea3c006afd9d0cb423440f630ebd9bee0359eb215ee2fbc115b5290596cd81a237b2c8fca7f53bdf7e922e102858aa5c" },
                { "is", "12d3d9f2d03cb4e1c1a7bc7abf2c2d6ffefe030402d2c8d5825676d6f55cefaffa19f330a4b2426486dc2c8b8a617bcafbcb0b78606e65334720860460cb1dbb" },
                { "it", "c6cd60078c6592941065a6b846c475c408dcff1f12baf037e81c9c9f33f41278f431f9dd54ee3b19dd446cdae31ad02bf1817e5692a19ff19f00d2f02484e8aa" },
                { "ja", "1852121f24f5b0775326517c45b10a8fcf133b608bb91f8dbf62bbac052fbccd748c838c8daa24356c68cdc7f81bc86b5736b3697a5b08289438a8989e270c7c" },
                { "ka", "e9265a7c7a2c7c0a59e6fca1a628308bc05b59c5ea85872c027b02cc8d74604330f5071ef7edeef5a961f1039f55aa1383fdc64248d6c341b231a42a8b2fd746" },
                { "kab", "70cc9d467cbc9efdfd8c52cc1ceb4e1f03e94a720aff99fa686feb1ea4bb80e0a0d37342271f711c9c3f91251525c9c604d82cecfa29c7782ee5a9b7ad73de38" },
                { "kk", "68d504fced86150db843484a83f56cd5c7f4d3e27e9393bd4b155aa1098aa7927906f8793de2e09172662045100df909fcfb284ce5a8a1e128c8abd9c11812f8" },
                { "ko", "fc7c5bd9b93021964c0d0f0fdb6d65ace6da0c39f704c04c0fcf2abdb94469c094a8d682187fa6e535608d4bd2973cc383a4a60f6e08fbfc2591a67ec1701084" },
                { "lt", "404f71ae05e6219da2177d733319547bb922e710487e031c8611e4b303ef9f3097c3a93d7a3ca2581dce22cd07539d2883d88ab02af37af5b395630903018438" },
                { "lv", "d074d71ec895ce509a20406b0bb714dc2a9c6a7ad5276fbb1f5dfefb3c92cb26e3942986d7d49dde401b3fdd1ea825778d63ed583b405e062ed3d2f35df76ceb" },
                { "ms", "bd41e52c15a8b9658e43b66f8c2bae8f4f94bfd3996e8e18b13540687f60d6df6fbd4375abfa47d7ab776438c9deb110f5bba38ef26b77c4dcddee10366d479b" },
                { "nb-NO", "b97537e978a4924368dae94754b908da4d396a2fdf04bafd2690eed5389ded8ab8a84607932091d0e236f08457c9981a5e0cbd574d76f45efed71c82693b4507" },
                { "nl", "2f93bbf4beb5cbb98854a5a26140f84b2cd9f5002f1759edcdf34f310ff2c723b06655f1ead1e8105e677a5e6e0a37da4db2887c026b86e32d98958479f6413c" },
                { "nn-NO", "e58db4049b498454ce01d8c7cca8ef04fb37396a5cfd3c759ef9ce98d98cbab85226726251e07997b84ad5a0c6be3c6f5f72a902f6fecdc5ba16d7f981fe5a2c" },
                { "pa-IN", "0bfd7748476ad2ff2f9daf02afc84595c38d4ff7f0b47746b36bf70e30702278b56c6f5cf7ee331b55e0e73cc488979f90076bc079847fcfb47257291b3ac615" },
                { "pl", "2144358c30144e0019ae78d74f55ded68626c82bdfb54b6c745c63575c58173fa16e6877a51ed39ea62761b50d17c758c3cffad22fc4ad55c4b953aa3438fde2" },
                { "pt-BR", "a51d6ae50746489716953e051e06514c5ee937f1614f101f85dd198db5079f49a9fdcac36cf7029c52001aad08066a78b5a2f4c5f0df79018c0bc6374fe0ed0c" },
                { "pt-PT", "7c2d379c3331f99c47403971415277e4d4d9b0c3c2e7e3c343be237647b27a5c26024ee1daa6b96b639a145be3a28581733d0324b748407ce1df2847004b64c2" },
                { "rm", "1556842dc8c98384e10278c7392205d316d1ad89630cf24397a6c53a00541890202e131a085ce926789bced517b23b88a2d505bb622f3dc0be05680ccc7d5cc2" },
                { "ro", "579b7bb1e3b61390e3245b5ffc847077765528aadb083bdecde6797703ba2c2b59331d06c20f7a1ab3724ae34cab363ba32f876de0be8e237ad7195f686f77bb" },
                { "ru", "fadb01f71dfd7b33d49ccac4675215b3cc2c325ac3df0cbb95fe3769226779cdf476d02335b6272ade61027005d850493f746a5fe25bac989cf53b2e493bc537" },
                { "sk", "dbdefe44f360db62e9a7e9e39bb82f492721b9fe09ed5fae48848b88b4faee903ae150c716df39507c613e754d23aae1e926ecaa91bcabce103849ec30fa057d" },
                { "sl", "360803bd722f37f7ada96f607c44ff22760de4d40b37ef4748d7b91b7a73f13e814cb6ec90481522c9be9a453a7484177c25b53901ec06f621d2e8d0c564d392" },
                { "sq", "c68481cdbb51fd68d7ebc301e7654504d2aeecf68e5d1142bc66cce5c9e257dea283e558adf0c70ca830ded2f35a2b7474bddf876ced31551410c1f2b24dc89b" },
                { "sr", "08cdb76d161d4e59ccd2606529657098d907e23503777ab9fab1828ef8446e6c33e331aef67fb418ee8c90645f8b3a1e41a5137b68141d20b07814fce012c5df" },
                { "sv-SE", "457e852d8bdab520f3805525b77ea7a92d494bee2dce6d1b36b8930548346b9d3392c085ffbb8eaea45219378c016c210974f5f5a9de47482a85da1d83031c9e" },
                { "th", "c561eb5a143c19ec2a9ce8ab36174f9796b0f2c6fdba5a7fa5ac1c79d3b779f8daa4410cf016b7660c9d0fafc4a44203928c1c987f41e61f4e57b791a7fe0f17" },
                { "tr", "13903c284213558e3767a148dea7e84eec4cb871d6a80b687556a05782ae1dedf8d3a855d8c79f4fb26df3190c6f72aac64d91d1b73c6838c098003511ff9b69" },
                { "uk", "6280149c8bc664a9ad6b4407f6981513d3a4c94a89316c94c3c3f4f319ab2a9b6d5b178cb0e433b06fb5906b65c7db7df196dbf1b31f78e71894351b5716dfe6" },
                { "uz", "76a87de47d03d65c46240428d67a4752dad24afd01ddf8febe251b6a07a429a26eb3d5051882fd222e93a2ed7ce8c5adb07e29ba4a9a35960bd011a0f0183c36" },
                { "vi", "69ba8bd205ed5f5be13e2230b5582ba699f97f1594d7a7c7c408e865470f32b6c05fc5b919abaffa3bdac48a19bf9f695806d282b335e74a5ab43e781272c7fe" },
                { "zh-CN", "ba538690ff99c43128c4264bac90d9834e5a5d5e40305031ccc401160ebe6766a93e562b71c78c3434c6b1df1d8cac8047735ea1bd5526752d8f6252bb2e6fd4" },
                { "zh-TW", "5c920b832d65df2991529e42289697b239638a82c32a9569b934bf17a4b787cd67bf32776296a4655b8fa7c4d851ec6280665fb04f5d437863c0ab0a928a3ba9" }
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
            const string version = "102.5.1";
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
