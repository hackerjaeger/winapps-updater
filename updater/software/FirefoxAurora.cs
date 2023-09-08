﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "118.0b6";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b6/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "fa4ed2005dccfe01b0fab71f57ff545d4749205181bfed9aa10078224325f03c1c4104d02a2ce5ddcca494cae1bde8f4b961a764bf33d2c348e76e34533cfce0" },
                { "af", "4f478312d8fd29e1997ca34f6ea9ea611998f209c52542bdd0676f00302e5e9e85a49ab9d0be5f533d8cc1d71346a9d43c69060b3a011e2044f4d9f989f594fc" },
                { "an", "76968fc9a4b3dc188d0e2e4d6951d3c652885d7e393757bb32e99e3e1301a6cf1b87b78428ced8e9455d5dfd0f9d8191045017675c74e1740d6c06c3979c744d" },
                { "ar", "a73c40d04b228f0eecd1b626db4413a6496328d7a96ad099b35930bdd342ef70e3ef21ac8bca1fa1cf854ffd44d0a52fac333c85f32aaa90d491246dd1456662" },
                { "ast", "b2b7dc5b0f1e13610d6674dbe91c26654e045180330f4b51ab357738d431d1d388cdc5738d056bd5c54dfb85aa15cf6270c41c0e7dbbc325da2d66d5661e6599" },
                { "az", "ca7f40b3e1f1d18f70249da7ccc6cb20a1bd6559f7e051d3a519e3be8f7f297fa2eac35975312c91f292551378a154a4b7505fca96845a284968d21b32cd3a27" },
                { "be", "fea0d28b6caaf3f5c86e347be66ef07024450e97c52ced6799be763b14af00fb7d0debf1acb72f64b896bedba3ea19807f8b255f3d52fa34befb7200be0baac1" },
                { "bg", "5ff170abff9522246f238d17864cf110003dcf91579c4608099ad634cddb2b402b4d6df9f93db1e624cd09ca940c208b3967f84b6c3d283a8d843197b90d8640" },
                { "bn", "2cbc0c0999fca4a046b2077c5444736ac8601666dd01600c00fe14e651cb0518b47c4ff32e93028795f908c6bf051e13c151a6de49ef7698d9a540b7eb0f645b" },
                { "br", "a68f1821e2ba3a77e7b4e67fa00efe07d3fa244a446cf0777568b4b7e9a3a685d722010315715c2e0189d4c19b563bfd9ac0f9acd20bc2d6bfd2651ff689d1af" },
                { "bs", "d0899295a6778fcb6368ab246385bc8ab548b9cd01db8795931e1c7436cdb258cf3d9c908d96cc5d53ed34edca2d4dee4682304d5d47dcce33a758edbf6a84f2" },
                { "ca", "36f3fe592649f3d40ca0aae7ad814d7a6769a9708bdd11111a41ac719d59b33a1a3c12fd6846f0bc808c508100f34b0650f94373d982ef98cf199b3b01210f60" },
                { "cak", "85f74db75ab1d893bc6769759f13b48a7eec8af5ce59a4fd7c938c23a0182bacdebb0fc59102232c8a6cc358a655e9f15a0886671bf3baa02c3f3e42ac991152" },
                { "cs", "0c205983eb23992ccdcdafedd21aff102e7209b478096149624729bc30f4675fe72abebd538d62ef2dcac2d4bdd86fbc8bca068cb9761351df0e7b58b33c4744" },
                { "cy", "8fe3a55abadc9dec64a6919b8d0c2de150edf1d98ce86b36606e49ceefde63264a3cddd75652b67ba36f899035c4ff8bcf9a4336ef81b8f998d7cdac707d06c2" },
                { "da", "f545a787a2912261e46c975617fa76481617c68e1022d0f053a08a6145ff55a4dc9d36dda6ed642e1e06a248633fcbb398499d78ceb67cd53336b48299cd7640" },
                { "de", "4a3fa90b2461087ae5e1a8c7f892f1ecffc6c411f47cda719080529f9bccc4abb799e3caa5237ab12f11638c513c6cad707b15f17df42d34ca4db978020062d1" },
                { "dsb", "fe6aee86d4751b564b1d4577baa617ad47040110f2213b8b0568c6f5f4f7080b5b6219534c39dff065526d8982e99fa75d4056f31e536db1713cc06485842113" },
                { "el", "43671b21b39e37c930b95151be85993f6f6aeabf226d8edac4c40f12780e6278c1bf6bf821d37adfa2003df5154fb76e2f40ed4e7303a014001ce56e1f79250e" },
                { "en-CA", "55abbb6e9adee2d6b911bdc7cd7a824f3645e73699e8db14bc98a206250920c32e0208269bad06ceef35874ff6075104ff9c967ca18fa69256baa18c52cb45b9" },
                { "en-GB", "338588adcb6954ff58cfdb7de78949ee701efce36f7b8422d6b1c2496999793d9266407afef8c06d66e913ed818be81d1fcf518d0eb81442d88ff885352254a5" },
                { "en-US", "726b4e996156b20e75e7ccd7aa2af9ab7cbfa89db46ac7012a0e3cd9696856dccc54f9b21b199d9fb4e1e5bb6827cf8cd7c59cff32af364809ce347ae43972f5" },
                { "eo", "5d0f21c30e1194770e4d4489dec9cb1f3d6011082d592059000e6e334ea38169c4876faf206c5c52ba9592ef8d381b1d0e6b37cc4c49d4abcf73572d3826e675" },
                { "es-AR", "f0f4609d73d7b4632dad6b085eae8fccb1bbaab7ab24732c19df62de678bf22d6f4c7e4b24b4895e123577fbdbe2da23f85feadb78359cf72d9a8ceeda31eaaf" },
                { "es-CL", "24e30f5afc806d6162daae6bea77809afae41bcbc5ed09f027927c2f3dfe14a05f47ced38538b8435f57b453c0e4e11401299ca4b2b807ea694ea61322c0b422" },
                { "es-ES", "31722a264a02817a1ac7215477dc6d839e9104a791c57beefef9025e1009109c07565ef62a6e0ff44067e8b9803b1f16fe883dcfcbeebc2962aaea2ac7f09a27" },
                { "es-MX", "259a1f5f6973de61ae462b829b8902205de5ca6138b756c285f2d6c8f7d923cdd4388f0f5b8ecdcd7697541a3d7e439e187f4e4c449429311dff37f461232f57" },
                { "et", "7ca3217b5d802642182c891bcf21193df0fe2a443ae0d5268a6808665f63f64e1a078327a0a61ccd998f9a5d2bc76ed1bcfb4b91f0c075a31f8274696d2d227e" },
                { "eu", "b642cd07eb6742b64e3d34eb90ce4cec85b3fd8c8fe798743251695bb74f938c9ccdab2026bfa6dfb93263be0b57b2ba0f6f9fa355ac094a5ee8a6dd88470946" },
                { "fa", "cd9974f0f570c13c97106db49d83023428fc40886c798165e24e42cb702da323b4c0688859053d153f5589285fc7a096a4fdd13f7d62a2f562a3e53e9f6ace2f" },
                { "ff", "ac11673ce5dbabffa8de5462afec4e39611cb8f213ad1fbdc645938282a5e7525557be222b41fc57a15300a1343fe9f233421af054ac8e26b3271426e718d05f" },
                { "fi", "9d0d20de0cfa7fb7d28cd7c98ad049e2eb170a5530f9256c1e01121292b42f8602667a7e233ad04ac58b30ea5578d4a0f373864affaf8be11f12057e6a5f23b0" },
                { "fr", "d73de33ba46abe8777874fa15dbaf6a9563277e8e56797508a23304737feaa7acbe8b14f758826baec7e6ff678ca1cc0980534b721254e7449bfcd8a4e5a989c" },
                { "fur", "93fcf509140b393fa611dcfcbbfd165ae33dec19f1c1e03e2a905dca30b70f9aac62247fde61d2c3121c89af57faf880efb11fd1f30375cb4728fc7789551f00" },
                { "fy-NL", "81d95c11c0cbccbc115e4e45e75a28e616926d194dce727b5a12536f478d0bd37b826121ed81ac45c48386f686c2dbb067fc440c762334e6713c458368d140c1" },
                { "ga-IE", "42985c406764ebd1e5c5fd318d7b749a293f86d75aed963c4308218bb08ac40d2a77e0e0e94649146dd9188229d399fa037d36da6ddb52eaf7625cb2bf249dd8" },
                { "gd", "ae7aaa9ee0adc089d9ee321aa2173359341c749d8d8c952f2f9f9fd76bab22c1fcb2e4855b833fd7deaf3a2645fba965a2bd98d018742b6420d71ae8fde2ba32" },
                { "gl", "dc4321c9fb4d7043bcc619045f7eeebdb71e5b419e87f692f69494fd32e3ada3a4a3df7755b48f17f36ccdc371e2f8097050f4137ea1b89def91701f4794f634" },
                { "gn", "74560664eab2dac1e85790dbac0bf2e792794acd6f0474e8b126b66fa63d7b841c36e1cef9f4b82187c53f4e1a632c9b70b49e21368729f39ae76e72614cf761" },
                { "gu-IN", "c7143dd10a04acd1aac724822b07a382e8da402edd8f14a99d7c5684f870047be772f51032a74a7d1428048335202e560d8d767211b1a73f143caf2f819fa116" },
                { "he", "20244e9e1aa8760b7b477eb19663b8cbece6ac0ae85f665926107f4624e4eb1b85a0b7112c26a7dbc38cc163e312e2d5410c467b4a51d7cfffb5ecc7dc0435ea" },
                { "hi-IN", "5a444944f1861cfed1a6b32992e3f0b2191734b5fd12993f6d1ac0cc32e7f7a004b8b62f3d769904316c7cb0a0948c3b950d81c0025398eb04dfb3c0d3a51894" },
                { "hr", "7341375e67a121c8124619d413e758fe8db58bec68cde9bd693fa3ddc9429c12be0e8392a661b8007f1484fd2ed57302080db788b44710aca2b1089a50b544ec" },
                { "hsb", "7389a52b3f5d9cce9d8bd147d072b6fee509413d4ce671ccd9ff8cda5f143b3eaad7e03b37ce07639a2b2791fa08b3416347bc049bd94b396b18857b5fe3df39" },
                { "hu", "0cbd8ccab5332925ffbffcab93d0ce57463eb5989f5a3778fa0f26adb782bacdc281dc3273570498593e8e71e0c33b34c099b561225ffa47ad3b646ba4449c00" },
                { "hy-AM", "df5f5c7ea7d6d770c066199f8d3aa541504816788d544fad2298b3e94b5363a2fe62bcd56521d263dd5bae21d7f6a4d0fb3cd6d4756ba948c792aa7ebaeff6de" },
                { "ia", "b02b346791c98fbcac62fcf8ba27549816d12a995fe936f4135f47c34d280c17cb4c9cb83a62fb17a38b4c86419065f1045b8cf27c75df54b741b47c6f6d2f8a" },
                { "id", "888b4d085fe67d23f64347efd134a095897834e9d5bb101786c8918f1a34558791146dcf8ff622d51a40e0e6e0a913d842c3985647e525cbe5612473c9106f49" },
                { "is", "a564df3f4bf4a9e8b4b3497e843906c3460063d87518e1cf5107896bcb7bd7af90148639dff4f192ae7dc1973710e410cb982673bcef96fdab9f632e9d1f247b" },
                { "it", "afa246346048ec2800bb90b84aaaaba0f07ab4de998275cffef52d954c35b9275b5f925d35b2fc6a9c05fd961482d9a60a93d8d213ad6981fdce5a84afa31b22" },
                { "ja", "ba92bccb600575ce112e7f62ede2d5aa615052da2fa5a2f03feb3a9cf301489cf2a2b3fe7c451cab5444fd900ea8762c7363a2f2838e5f18a32e977bce518aac" },
                { "ka", "c50e1c8d0b007a8c6595df06c8e0e5095b239d9a618a4f621d7edd46aee1e59c367cb17851dd1183fd6f6eb225845650e9a0bd8a7afa46951e5d2b8aeb959c00" },
                { "kab", "c798381e52d285fd0c2e09f9abb41633e7d3aa256e3d645bf785e35edb0b5ac66570b3dfe9f8e2167d087c79503b59db83571e93ff4f8bbf6930aff7c0277a28" },
                { "kk", "ecf584e5f55e2df2a262d84857b1a208f773270ec5374ef953732be3e25c05be0bcecbf82fb1e9e90c0434d8cec0576ae972b3d14fa947ed63716987525f8bc4" },
                { "km", "71949a617c3c44368f2eb6791cb14e8321e54316c995949cdb5c024fe5bdeb145c3b202234eb6fa0092e52739f21f341c0e9bf29d33d9ee1a20e545ba88f0168" },
                { "kn", "cb9bea285607dcffaaf392d734f9d9518e4aa8be4b5ea1939498eaaed352f3a7323efa3abca7733b18de127853363a6196141fcf77fed3958f294b1b2ab50d10" },
                { "ko", "8cfa27c685cc0ed1fc69937174f3f8e6e7f6d0dffe2040e63ab9f7cb969c93236525bf7fb0965138ea4447a8a3a863f30e97b8d7ced95a59305454bd15fb3196" },
                { "lij", "982ddbce5f46877e10eaa8952c660c4e34e7211b2ccdc1d552f0d28054a57a864c3cbf4a9c7b5e37e009ede872fdbd76eef749f89e73cb9d8ad43f36a1994d24" },
                { "lt", "4e9382c18d35cb7c850dffd759971645c562c7c195f4c810ea9b012e3e12ffbdb66e73933d372f8f6383cac9a07e063e026722bb9153dfbbe9989c0f196bb783" },
                { "lv", "1c253a502bb1525a73cde5393b111cf953bfa33e5bd400f6f513fda56b0ffd91eb0108d60cb455742ac599bbf8d3e5dfc0d9dedb6e82ceecf4980707942e83a0" },
                { "mk", "ad816ba1e1d12b9a19deddebf0abdb4694cf78ad7255047214aeb7827b330126e1527e4781637a6056a92247e9defbfe9b6a3dd7fdda1032955faaa5ec635790" },
                { "mr", "7be26b9a790f3f1c3078f253c816ee47b6d53c04f654c3d316c565daac00c46536d958b27ef23c3c6c467710547c603273684e3718f1db97adcf5d665439c5bb" },
                { "ms", "fe26a892deeba04863f7310a5399f9fdd7dd7f93a4acbb6d0dc71b6cd34e61d75cd883932bfd0655c1a61aa5676f2dbaf9a7ebcfb7ba852cde4e27af0a2a1d82" },
                { "my", "011320e6814f901c36c4f282818de886b2c31c22d07129356d1aa07bb74ff935694e10c37c0dac67250738333e1ea4fa19a670e67aa3e66fcce11bd54c1bdf0b" },
                { "nb-NO", "b7b836ee673ebd2cf99b53084bbfde1b22076435e028b49e139a7ce5bd7728eb29487f8ab06fc595d1fabcd79ed79fcd827a6acecdc5b30b67cb847f874fa2c4" },
                { "ne-NP", "bfaca61797c95a54b186438d1ae0f6801ffebb22f9470e4091e83cdaa9f9adb8e899069ed6b4982ec60170401c9ec3310e983b2a4f2fde15e800284e48f6fc99" },
                { "nl", "98624523d896135932de8e526a63efd94ab980bf0fae7fbb82aa91e6bf3b793032c57cb4c95952e24d536e887ffa99d7db94a2f052384ea2ad95bdaa9077958d" },
                { "nn-NO", "0caef0a75994f8a32a5c401e7ef8e9c8a7642f8f24359b539d0f81e2c0a98c1d7a90b5c2d98183d75429ba77c08689e2956e21630c1b2c0db5e2f7e5ffa6c946" },
                { "oc", "c90674f6c8ddfb9371034e5b488e4c792338a3229db720ee9cb59b2b3f3cc19f29ab340d07373f8c749fc7e8d8269307a9d8977dc9039eed3f499d98e9d24d5f" },
                { "pa-IN", "f9ba62446104ecd830dfe85c998aadfee220ac1b98945b47cd9c11ce2e0c2539908779c39d0d9931a54abbc1845364ec46fa0b77bcca23b41adeb6f27d947ea0" },
                { "pl", "1a868529d9f6c0069e98a36720a8f90d1c905885d9762b3bfb8b0eee1f26d6cc0fc372b38dab21c93ad64c1278787e2b5ab8115714e8c84010acdb4c45b5e566" },
                { "pt-BR", "8f40019ff495600092b594e36f81d2b702fbc5e9bd2820cc769bae2bed75bb82f28cb051006253461b6e8954d836c272269c7296914e8055fa0a2d7a495a8c97" },
                { "pt-PT", "79a618eb16548b8875c564de8f9cacde8a4defc74f6eb8a4cb6c9339f9443c8258fe4083dee0de8218ad578ee1dc972fa118c3a079234a7179f441cb73110aaa" },
                { "rm", "ddaa154b59e91c10ce3bab443644af286e5aa2f5be7cb03635e690397ea7f7a9df367702c3ff59a5e5855cd0b47dee7797c352a61ebf184ba9a1c683a3755077" },
                { "ro", "f2c390439f3e4c224d4f72f19cf20df4103793f8dbb68f0b8ae068737d14c377aa94986c79fecac26493156af407e3b4ba4324e9d3d799e897d5095a36f99d19" },
                { "ru", "eb9b300bbb89eab2f1a98cc8ad6a9910e33908f2b678a2a43296618acdd25ad6b3b8eeeb452a38b8bd08f0de890a857e353dcac68d014a1d4f234b6f17bf47dc" },
                { "sc", "6f3ad1c902f3f0ef2b44e532f60610f81dbf14513cf416724fb16947ca1831b81d21e26bf59620cea03f1dfd52cbf7032109985316f36a8194dcfe1f112b484e" },
                { "sco", "0224eca2edd799bb3266f1f354d6763cf7c5129c70d93eb657bbb8c0e3b2ca4db6af2cab0d3cd0bbcbc6b1384ea6a5533bb1fe7cc3f774fa44d6c26a4edc4098" },
                { "si", "b2301c6f85ea36ef00a662bfd75d6d61aa87adabece31e3cf8564aa012e64cf0293955e807653492deb4d84e679dcede18c9819ff9cfd36c23a7b01bb398a833" },
                { "sk", "85f5bf54b413899a51a57eae6e090ef4657842e96890fb73e192a74f0e0efcc437c3bb3bb764bf5ac366a5b7293b2a59fbe46d2c31d61fb6a3ddee9e4297de48" },
                { "sl", "7640b9fcebedd4388651d228139dc68c56a14ce69c0259a937b97c3c9808a10b0b5d5317203a608ef49fda58b1a7ca40bd033853a75f0c6f5021b543270868a1" },
                { "son", "851b14f7f7ad86929c6a806789d339730272509c3442e235aca5529f6e2e1706672e148c2c30e9494ae7a5014cd37c65a11b41b3fba5404103d68638e0a852c4" },
                { "sq", "e8278589693c0b996094bbd52bfb560cb9abb331b629aa335e06b7ddce319724fff96c67ffee85be4c0966aececb109530ad30993a80768d3855180f019873c6" },
                { "sr", "881df56ef1ed49ce0379a6f6e5d6fadda48ccd974127de5af2457d1c51ff2e86f9c156c99a0e57828c854728d6483047edf6e71ffcd7aabb3674a38ac361d5c7" },
                { "sv-SE", "7d37480637847671548b169be088a54fb8e76661046e1c4877666468c94831e9fdc54b6f66aa5e1194a6d4e7043c82eb8499dcccf6b07d5cf186ffddb0bde338" },
                { "szl", "5b09b99f986f1d7bddd478648b625d1859db8736aeaaf547b263513eb454e507acff51f4769e02f371f13d382675244cd6f3c8361ba6e7f390c960f7d47852a0" },
                { "ta", "52fe5a9c5af52c82ed39e2021afc474b8e60f2c78ef2d5b6260b2dcabe1bea18281dde120a3925ee6c2688c967a8af08abc02310a2f3943632ed7ba0535584fc" },
                { "te", "e11373c7fd6002ea7cdd410ff22e96adff398c184544b61ae493e2d86246d9bbf2dd38641ef1a3267a02d847d22292c861cc7e9e331e911a86ea2b9996aef8ce" },
                { "tg", "790002cb2912a81d73737ef4b786d6c85974b65d1ae4064156102eac8d432dae3c9dc038ac32d9152bec3e5ad259941d7a8046a7977194fe341839fa1d229aa7" },
                { "th", "55148e6c927e731ec2ed6d00621b514bd17d8e4041ec7e5012787d59253592c4408ad0e95fa2283aa9ff9e0aab4ae6616b9e4c6715777ded3ee7f888a7ac3835" },
                { "tl", "e37846298e62d148e518f67de04a8a28b81f5426755a23a44003211fbb1fd6f2e28c8556c294d4ed25f3d3797b3009bc7c1c2686e81a360de77da1a0e8b0ebd2" },
                { "tr", "cabda2c6494d5a01587fc3dd52c2928709a47fa561b14dd7c81f9462e65b57f3ecd8e2b3f7f7f1e0374346e58f71fe44c1dddea1dbb66a5db81d7ebb212b799a" },
                { "trs", "d353a165282224d6cced1f7c0bb4164c8579f29458df8730c051af492461a68c9e85229b402195b1d5e7d6faa70a2b45c8bee31173170d13384c165a270f1b25" },
                { "uk", "394e5ca1f5735755e71ec60255f276fea35acb0a5a99722988f14a630a6f5781a62f672bb126a348fb0b9da2d4e6a35bb9f383bd11f692c9e8f49b553aa826de" },
                { "ur", "717f4f621c7ac8b74d70a23dc2da4ec4e37aa46eac707063eb06e1a696a81afb17f39f907b8d9a78d6b9cda8f65dbd994aa1fce77650299709296d20885e1a58" },
                { "uz", "3adefb3e2cfc959f39f9df21aa84fac0ac5bc4a46b0edb520027e01c54e48b60e2608ae2ad79ee520036e9cf564d68cd7768f0a2c93166c6d41aa66bc1f9d84b" },
                { "vi", "f920725a166280a21770adeabcc2573d6951e7f521426713b8fca94d9dd6d8f77ba48b08a838987a7106f364a320069f7f2171cb93c86902d5930a6a2b226e24" },
                { "xh", "91c18141d7b2e0f067ac9e7aea07d759e98edabdedae7ed1aec34e53878ccc09027728f4ce60d7f14d31d6682a777402c822c9558ed6ba6441b5c413de5f3e0b" },
                { "zh-CN", "87ec20b76c0b1e5826c0cf27d3a768a564629cd4e9f200a46b3e3785895a5c77b4b5ee40473cd1772ee117c6fffef989b923ec285d89f311ad10ff830d4d5508" },
                { "zh-TW", "05433fda0bde9771045568e45300600a6848f50a0078bb844da9b48e7ede8e4091e4b8c95c238db52db53bb5e7586ffed6533ab4e4dd35ec914be5c8d4b1565b" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b6/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "577ecf34189c7ba571079e60b0744a69d03226899917bdf602d4c7500cffa9ad1b5acb394c898ae067a4f2ebfa9cb96b662497b4b5628ecfd2b7d65b6dbda8d1" },
                { "af", "fad3ac1ff88e3c45e653212e5953cdcf159b729921bdc091fc21fe52d291cc591ba4e7608d5ce4514989787bae24b4eb77a23956be60cf1e5e98b0d42fd4f450" },
                { "an", "1f2a141d9153726308b63abb54e2c49f9d2d2e6383dbc912d08b8bfbbb29e837bd6e1f3ddc289c9c7e58d13db8fe3dfcf5dba908727d1120fd654cb5f9b5eb72" },
                { "ar", "7ccc89ec256f1ac9b202e48179ba6ac9b7387a736b4b08b564281b446af755ebacf3450254fd8b98c39bdbfc87044d2c677c478f531e671c0395970be004742f" },
                { "ast", "76e6301befbb764f11942c0178de07cd3fbee445af3650da8a92090f5526aef2c431e085789cab6377e5fe9b4a760de38a71f80092091645b59e6c1f5ee0e321" },
                { "az", "3b38b161b7ec44256065771c1962f94d1310704957b1022df23fef6daf2fa1ec5d5beec67f930afefcd67fb9cf094729b938c355391205218f161d983819c5ca" },
                { "be", "053c33e73ca5be871aeed31d0772ce020c73b132a5f0627429f72e8aa98ba8154833e99b322ac5edf3b6e87db28492eeae794fdde6faa470c1ed9efadb2f7227" },
                { "bg", "25c69b6f26d25f0d6dce1930f0c1d1d4c6734e9fbff3293555998cee6019f26113ca840254f25c56f707e51fe6db1f096fc702805d62b9af62b9240aa218d666" },
                { "bn", "e025614dd73371c6ed46baad9e11eef2bab4ac8ce8fd1c13cb82a25ce12c5d059d1155143f378d5ef0faf45c805e411c6bb1fd5d24aab4e075c4e34698698d20" },
                { "br", "77acc4c74c6e473e27f2fb10e19e13aa175102acb1a46bb1eaf3db54f22cff96d07faf747cef923807bb6290614b4964da7b6fc7a7bd2c0e4a54fd506e7dea01" },
                { "bs", "62c34baffdc7c3417a882d42213204ebd676a0fe2deba0cf84e8a348140e2abbb19b6d32200d59e66a592b9911f1192c23f8e9af8d4a2935a6aba5bbae838df2" },
                { "ca", "6ad41027a43c0fe270a01212f1b66215bcc6350abf516e9b848791ec7c1197314a560d3c80ee973314e50f7ed05fe2f6db6b1b559346a581f522953fd2322186" },
                { "cak", "588dc994e7b37288de0872a5696df62adcdb5f36e11b29408bb88fab42fa2045a5de9309f86de687265193ef842acd680968d620b84a86306741a12bc2ab2539" },
                { "cs", "445566e94397c3150afe1691a2284d8ec22c5d9d78ed67f3573ed02147b0e5498532d01831281a8416f6d924e7c5459567aa92386b0fff5c168b0a2e1c6454df" },
                { "cy", "349fd9426e1d14fc3075bee3456c14379464a58b655b9c3a7c774b148d39a859fd3d8dc8a04640e19fd6400948c742edeb8d3a9f8570ffca57077141095860c6" },
                { "da", "6bc2d3b4e956250a7090dbe01641f6ea4c263d209f4edf30668bd03c65966258b94e5178a6565c29b27e0ff5ceaa9ae079e3a0e0f0dac29f5707ce8b4e3f4500" },
                { "de", "f8f41fdb168b17c421445dec93e630c5a283049bb41c02c0f8ad658a8ca2c6c25ece4f842162efbc99369f64c5b613985171703d424e7b7f93b8443f581ef954" },
                { "dsb", "612791d4e0b34aa92220b1b90cf0ba097fbd5771c5cd2438ec5e376b6fbdb33c44135a854cdfb10d03f160790eb3c300cdcce48a5a92c1b17e748a490466c829" },
                { "el", "416f84f5cb00e0c1c6a7428ab317a7f3e2e77f67cbb3e48d9cd596c0d92eb80984c900900778360b2c11fd8b6b334b206b4666674271ef3aafde34c6c433f3bd" },
                { "en-CA", "14b746385d6eeafa549e846771a0891bb48f9de687c20aea62ff5fcfe5bccaea4396b536b69a1bb0a066928f1dbcb3c95e5fd0b269467d572f1b6b9ec1c5e878" },
                { "en-GB", "514d8f3f9af2f8bdd2f1688f0ea37674e18972a5bf8e8b1dc1ee85ca7ae7de43b3d1821ad84e0c68a449ba69dd200a9e244d671d04d530280dfdf4a3f0048e41" },
                { "en-US", "72b9801205a91f02d309c3be2943ebad50997146cb7f31c06a1184a37dc4b234b1bb7b339d308fe01c9e1ae7239d3a2a9266bcaa6fd9924a6b87bacf8446a89f" },
                { "eo", "6cc4bd6599bda4b2a5a8b984e1f855163403ea51e7d105d977ec36fce59fefd34f189c2cfb3a77984e779827415d5532e0116295f5e0215e231f28251f642e90" },
                { "es-AR", "a4c09c26935a544c445e73a5e95f3199987ed3938008aaf3d6691a7194fe80c35b260fa99c080edbdc860c3f87498e3df76d4a5d1ac16b34148e2a9f38e0fbfa" },
                { "es-CL", "4413486aa207bd91b2fcc9834500281415b7016c2b39131d58235f0952a416f2c932ca01bfa8ee490b65110c9c47850883e0dc90a73b35d31809c336b44028d1" },
                { "es-ES", "2e7c28d5dad6b941b9cd87c3980827d7e8a514347d8a45dee0a908ced7008ba74dd8bee4db7104d9c2ad6eb4e1e7cd746d1213c12f26e1c8ff2169e3c8288155" },
                { "es-MX", "70bc8c620e72de6923a61aca71ccac2503d90cbc4353873af5e6c0b81b1300e80378d861475fa4e38e9514119be1c4eb65d859be7a8fc1e09d99805c73eba08e" },
                { "et", "8d147eba56152713674e86a1a5587804ebe4313eb2d124b379c5de07ae387ed14b80a192ba21f79e4f45cb22eb4f0a4b57ab9df366615bb430d61afd8c6b0363" },
                { "eu", "0c34ade0fd01ecd836b5266305dc28610b597f9d205c5e99d7874d6b4610c3736584a0542550b9d577ee0ad2e8a5a11dcedfc0ad2fc05d2ccb860551860e87dc" },
                { "fa", "16dbddc547b0eaebb58d990edec556fa7db16bc841ab254715cf9f8519c157c90abca42b666d62c7a233b72864d45a7e293208bd020b0d0c3ebf97984a01f34e" },
                { "ff", "b4268df05c67e6a48b3e0f3c8ecdf3d061d356247255b2d636abcfda56b6e1e34891a70779fafc89ce9890ec7d55900e773192804179f80816fafbea6a3278fb" },
                { "fi", "609557eb066680e801270a40a8bab7f12fb39cd4255874d22993f4e1bcd3b7786b5098577003daa2ba67ed329a23c01b878c8f09f9baa817073cd82839a7238b" },
                { "fr", "5463e0376eb06bf596611f2e17cc5c886f1b6b1a7aa29b86fa5f63748fa3df753a9084e8a924b22dfc3afd40f0e029aa07e9f61c5f4823db64ac06f72edb8c91" },
                { "fur", "33b287cf852a9e4b1ff41dd670e6ef1f95d8606da9e1973ac6f02c322ed6ba20fb10472cf69e2aaecb52e34399a0dac6f6228d0c08c95f803dc6237382a4a651" },
                { "fy-NL", "e6ea1dadec135d086b9230ba6cf085dee6f4bea224f84af24cd2c8fe5737dd4529ef4420f7491e7bcd2460ae9e1faf00080acc1617156c2b65d9fb45a1ef9bcf" },
                { "ga-IE", "2f8f4a616c3b89bff17eea17aae990bcfcc071367da9cfd6c6910b5af919255011c4087aecbf1358b8489c9f78e82c406691a185b3db5cd5cabfa4a7bfcf4ca8" },
                { "gd", "33e17c1a35c724b012a58646334d2af743fd6fe6de939efb8621edd445149db3c7eabd3defa0ee25105bf33d127b700b1313d163a318fdfb5327f6d04c163cd6" },
                { "gl", "2c1493d22250e4240bd57ed7a04658b151cb7f034378c4b44d178eab0aa0f7878f3e47aed526762d829cb7808d6f72b1a51e85279f3321c38d80447cfa966055" },
                { "gn", "19e7e2f31b67f65b8cae2e9a17c77af66979f53e7025335e42b6312885c3b6a6262c12541dd485d854e4498716a9b276107e02734efff20421c4e0bfbe9137ae" },
                { "gu-IN", "d74cff60a4349d57f2a383e8e4e96f3990c319e461f3a97ce8ed80150a785f4f930b06fc76d6c49991ed791c208090cc66da2c3059d7e19aacd9f4daeac9bd85" },
                { "he", "a10ab0e466b76e41bdfabc9e435aa12bb16ce1d503edb825e89c134db1a4944c2bb7e5f8890a12d465761206dcbb9d425d8a87196e87a60107e24d7f5292d15e" },
                { "hi-IN", "02659084d331c386ed1958e18d0d38158654a6df65996047abb112aea773cb869c7ad5170c77776a3ec2aed35a3e4016c65ceb9d6fbf4c2d0fb131cdf46b36dc" },
                { "hr", "1d483badaa3d3f35b6260ef432e8e2c77a87be4bb4f0922ad7c57c8be3a27ff78e69c6c676b808a696a68deb955a2b117ec5434e2e8023e66cfdbb38ac8737aa" },
                { "hsb", "38e42b79a8e241c2e4cc392c45caf1afddccfc0a8de4adbad0d6a1376a68534f3598d4e2131b3514da8784f1c9c1b05baf73da86d7f484e747fce3af3aec9d0c" },
                { "hu", "25bd18313df835c2f4f210a2fba1b82fafbb08c769aa5cbf0db0f9120b373acbd6a4a1b21bdecc2e5a942dd2ff6ce1bf54a8466a1a1b5c524b20c91cf05b1015" },
                { "hy-AM", "8d086ea119f995569d594a61a6a05b70124a8be6a50f6d5fbc23e4969c2a08d97637a2e3e45bf42e16c9abd7d9d0a76232d8d45d23674ee3c7f941df618fb697" },
                { "ia", "68cdb4f672d2ef9e4bd4c6e84cd1f1959169fce87b351f2d8db210eaa49d2fd6b91a7fc698f3bf8b23ea5cd85cd309ada94471d7dd7c5ceab64b94110c45e3b6" },
                { "id", "cdb52d541cb7c789783d4d1459d8c0d594af902f6abc7f709a45f61039752551121ac9adcec19bfdf4d0c8a53b08487e5186a77f164acc2560cc6e7626a35b55" },
                { "is", "2ccead5b8f107ac2ac21e6b92e7f2e5804010291b22eba42206a74e354209e6de4433eaabd00f1125f366fdb0b61c3c623379d4d083d38d1172070201e64b75d" },
                { "it", "63753d3d8faf895fbadb41745991872af54d41db139fcfbdd7145238083efa77a8b0fa87eaa2619ffbaabe8d201b7e62911807fd48ca067e6d907717adf184a8" },
                { "ja", "4876e50da4a1bc6189710992892ba910a30301b08b57e8cbdb34db1ab356d207de049e14634452126f1f3ea89edd91c9f0d25811e6a69f03d144e71a2804401e" },
                { "ka", "87dc6da795e158ba769ae2671f2afbc924aa79d7061709cad2c6cc19e8df25850461cd0b0ce9f364044f21a68d23555d8c20efda12a29b0b57cd769d5052c82d" },
                { "kab", "34c24c1969370cd2e5f3689e20474ce2ece79aa6e14bf4de8c09a7bacb28e74fd4a6a9f7f51cc40f859a2be86a6c39b0715785dfd0cb39da6d512af5e34fd3fa" },
                { "kk", "fda349e767f327c94fd8564ba73d6053f67a19a6eba2d027ff39bdd8a6ff8f2061927692729f4d6f7d9df05bbf4f5255cebbefa46d78c700097c963ae278de9a" },
                { "km", "10096aef23b24158d1d73a7d06c07d25c539223e3d62d04b3fae6d6ac6af5794c60fb92fe0cbfe19e7ddd1ac7dca50ef346fd4b46de1c26dbe7cf28b1612aafa" },
                { "kn", "af06ea3d0bf96fbef564f6f3285ff607838f6ce4f9081b5c28fc2836a1569d67ddfc072607863b5d30a4e81c436aedbef9928a8e52c2b34dc1d81d6d364feade" },
                { "ko", "98df348f1c3b94b22b1edd00df50564ddd9f7f52163bf59d5aa63fc732f3883cf2ec02a9b44e89aab6fc45eb752b9b4371fa7cc7effba6a191f2f09487daff66" },
                { "lij", "34c7b7ed6aa3f524b71ca9b08963d19fb95aeb50e511785d14f52b6ffa7e0e69f10b5d577c5d309b84893b44782a2e4d036e393fc0040f1ba50f6cc9e7f737f7" },
                { "lt", "44aa79b1b5e9be4a6d92a4ac46729f3a0095cc899c4b5a05cadd08b889a7e212d0b8b2a87e7d681fd8f5143cbdb0724ba81d2466152e29fcd3b6dec5e6958199" },
                { "lv", "25ae16d8ed0fc7d3d7fb83b47ebfed4222459b6a9faa356e80ae01d7c0212a4025d8f09e36417349760f9a890a793e2c751b7999e562a726b136ec054a6c4b6a" },
                { "mk", "2356db1453c271c3693f198032f605f52363983743b3e5f7aba811f036d7b3f04d699187b5d08b16368609a7724d0b2980aa3983f0acb440ff297af9ea352f10" },
                { "mr", "bcbc3cbcd6052585b679547946b024c8e004b80c67a4871f57a3d8491403a3f433757be47b61761e2bce6d57e81f24ceb65ad42cb3e130b79c46a13a0433df9e" },
                { "ms", "8648ab97453a23855054d4b0e47ed255511b5803ffee851b513a7f1bc31e83f06ebb20ddfcb0eae7d6007eaeb3edef2f69c92909954ab422a31e65f8df4d3afa" },
                { "my", "bdca4ecd14667cdbffe759c25109312fd7790b5e98d6c85a020dffe86950353756cd2030ece3daed55d33d81592abc8e1bda9d4cfb10e1064127c6bfd0b51d6e" },
                { "nb-NO", "580a09720bb956d0be17e76e6191832c9b1c763c9bd900de67581a9258a5a69ee732e32597fc585245b4e29b1654ab53c402b04385996725f1840b1efe81885e" },
                { "ne-NP", "abb71a498037a4f4afc081993f0e48c78422fb826827f013b62f7d8cc43bdee99204d7fd52a4f10353db7345378622da47ee024867abe1a7bdda399b597db2ac" },
                { "nl", "ca1884905105e837e40ad3eea3c6b17b46e79156fb8f11f54a0b0c51b69631f7c3b24966563aa9de8cfdb66cfb99e45400085246063108c2f1ea679188d0e17d" },
                { "nn-NO", "7408df41c1a788ced6811f63191d8a10d0420b8c2c5e527a84c949b5d3c5d052703b48e0d028693ae745fdf8fe26ffa71f0e72f735f72641a739070fb92c9d43" },
                { "oc", "1ec22e2fca539e5ecdb175e81e32b0e37fc0b4aa639937d2660a4a6e503eaa27623bee145a43f52b581fb820932c5a720279a81556ecd96763f5e9fcf284ed6c" },
                { "pa-IN", "34d4312107b99e22ca1fabc1c1181b73828a84a97b8770a76d86af61353143745d974e7f898cd8475f697e9943a3751fc20335966f43f410935fe883595a29b2" },
                { "pl", "71937037f3bb42d09156a8e2e64ee7967817374a36fbefedda7bc728239ea3c9c2a8fc987b502d4fa6b18d2bb08d1c17462faa1de1b4894bff08f014599e95d8" },
                { "pt-BR", "6e720027417c7d748c42477fa55bbbf0b6b92ffe627a370d799af9fa8dd16226a180b9d538f5e4b90c37ee4558f4026095f2a1f10f80932b75a6f5828455b6f2" },
                { "pt-PT", "4281575c9217964bd86a3fc7c5f351e277148e784a71c17df0119a26ba178bc97c126eaa2003939f2a735f6e5d322f61e058d1e3ec47eb2637533b8c4c2c667a" },
                { "rm", "74b21c1fdf30e3759ba64164e486d94f44ea6b574fbba0f01a70e2ea5f92775f3b632535e7e372a52caf75fbe50573c2b8a06809dbca588c8e35b2244b600b19" },
                { "ro", "92e8c69faa1dbdbeae3d2d7937359f8713162a7a9ab3af780c8057f30a90d128c9dba590ad0798880bf4cd3ca80ccbd8dd57fdc18cc556ffee58a5d42ccbb5e7" },
                { "ru", "b3df83e1079c8aa6ceb124df2e09ee36e4275c693c18c6e262e2079b6c11e6376bec6b1fec34253413d40c33a2a4390dc6f6d9a9c3dab62c0b0664ae5e085e53" },
                { "sc", "b54c114d0176cb8b08dbb0d34045c667c9558ab7ae86b57108f62273940f4cfc7b09698fb3975555a2b113e81429f0b30ed69abe4f28cd799831de21993900e2" },
                { "sco", "8abeba8ad13611434871a82da5972024691b3810faf5bf8d1cc28297799c4b640a7119676b0a541b47809d1cdf01dff521a8f52d4d1bd1b9421432140346fd55" },
                { "si", "a1f8867469d4f9cbe59fb02062c53c32ea961a350809861504b7a68fc20e9100e26f60c4e6b361131f125d360bd433a511fe0f9ffae0722543ce5008bed93477" },
                { "sk", "064268da34485cea68e9b759b9d62d7d519280b3cb568b2738eb85ce719120778e8b9e7e5af8af95c3d3b764d28949cf469059d0d4b23a137b46072eef7cb54c" },
                { "sl", "5e888408e24878b017a4b89001736b5dfee9828fb6ce1bb1b3b8a15a68bd7d26ee85ba48db74421bcd822482d0cab6842811dec68656e87195940e280c47cc8f" },
                { "son", "c5359fca7501e0e7996b7c47e57d4d92d1f222bd45ba129e029243530feb26eda22f21d440e6da024e148d5557d9be94b417c7c266ca23617e19f05411ba14e0" },
                { "sq", "d74aace4cca68c50793726804858acf299e95431569c198b6a34956864326ea0cb9512d613384eb8061e43304061590006447f8f7c343ef2fe21128d497f28a0" },
                { "sr", "1c0adeaf02d8d2125958740c8dc1c0e118dd350c8a4ef56ab495edc924622456adc4a566c8fe5c9dd18b19d71a6556f085a92c4eb815a541e13075eea291d1e7" },
                { "sv-SE", "95336eee4d40bfe79b3e19b461637df803159f6d93e9e16ff12f86009248bcee8a4f7fd00331f0473e69fe2a6a3be408e2ba6a6b30789d83a15434852d8600cb" },
                { "szl", "d720eab06eb8ed6516b27e64cfa3348538a683cc6a7be4262dc96e63b4a7d30d2791d2af9e43640827ca11a8d3f668f0ce61b25d1939d0aac017e3a60e213810" },
                { "ta", "6b234c6e12710a5154af01a043589bdfc0bcb6a4a5fd2d65fe1f3579b39fe7a76c318df73c99ae49b6153d6b5cfd787b7f213b36748bda5dac5a6a2f9db86073" },
                { "te", "af1c38238d7448c2d404267e8ea7c7e8278a1c009368b69cf47519c4237b8e60a5190f50de229d5a7fd3d373cc819b02e18ded1d8fa1d271f4634623b784a4f7" },
                { "tg", "e786453b4f37b7dc4949210b9a0e9e445d40af687f0e7e96c868b58d50ae51b379c61d75615efa914275e5b509ac074bfbf7f62c11c0b1ebb77627d5c1e44fd2" },
                { "th", "bbb8f8ed270345d475e67285f454c020968ea531f359247778150b6addccda9de92b5d77f4d8632468bdf7a35d5b5d417ea4fd919c33022c73f98d38a8bcea2b" },
                { "tl", "fdcd7267b7671be3d4f5af3d5984964fb9a51803ac5b70bd5c559cc79589a8606ab0cf93a05ab961f3a1bb17567e152a258703726253d39bd81d35d43471cdf3" },
                { "tr", "a1733c0ec064db21eb3681d1a3f440a404cb84ee96634f7954765fc4b8b65743784db2931329c9800677acfd5a862253f8aa4ac60e8380886a09488d768b244d" },
                { "trs", "28dd01212dce656dea102b32354ee5b8eb8cb93d2e1091d64403db64aa8ff54c9c5704fd0393c1fe1102cbab6b9b80754b02b0ff5a27976ffd05ea0139b23a53" },
                { "uk", "0797757077b8aacf230b9ba486c9c42cb664f6b4161ead4698b537e388518bbfaf78b04b3fc02f2bc04652a89a219d277fd572c3e1d9306e5f593f169a9b8a5b" },
                { "ur", "96ccb1c9455d4b9270a6bf45126838118174af16675728d6fa060659ba67dfb5cd89f203ded2cb7fb55419216d04e1b25276322ea29c28611ee8eb0bd06fcf6c" },
                { "uz", "d1eed2127adbcf594393c15d0c31a8dfbf74516a979a15936d982b8de92a877bd0a0265471c34fec48c5b2739af6ffc527d6c99f9b2f6b318218cd49ba56a493" },
                { "vi", "05b4c804b5b506ad2b97cdd1ac0f0eb55dc4e984eab6ce59a9370a1099881e28c74817dac4af3a3f502bb4b44f58bfa2568933f359c13cea204216ad35f9557b" },
                { "xh", "babbe4bcff5a68489db8bd8fa19749e45dc66599e5bcdb47620299023d386f7e698c2ce4cd444e0654662d111c780b38f63a2527f33c1ef72b20969f77a64e6b" },
                { "zh-CN", "a0e8479aca45f8beda94e9e6416c1360f10a069019c2647f9acaee2c808b926d2d48393bb0a8b2c28a3e4229a30806efb0b344bdb27c6a5c422b6dfed80ae698" },
                { "zh-TW", "ac88e762a0078de865bfc6e5655e7e1193738094515abf6c6c30d48e79b4c9088f48862034e252912e04e125debd8f3b5cfa499c15ede2b4a37fddf29bbb7d3a" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
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
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
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


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
